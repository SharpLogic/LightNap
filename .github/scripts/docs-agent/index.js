#!/usr/bin/env node

/**
 * LightNap Documentation Agent
 *
 * This script analyzes code changes in the repository and proposes documentation updates.
 * It runs after merges to main and creates a pull request with suggested doc changes.
 */

import { Octokit } from "@octokit/rest";
import OpenAI from "openai";
import Anthropic from "@anthropic-ai/sdk";
import path from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Configuration from environment variables
const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
const OPENAI_API_KEY = process.env.OPENAI_API_KEY;
const ANTHROPIC_API_KEY = process.env.ANTHROPIC_API_KEY;
const AI_PROVIDER =
  process.env.AI_PROVIDER || (ANTHROPIC_API_KEY ? "anthropic" : "openai");
const GITHUB_REPOSITORY =
  process.env.GITHUB_REPOSITORY || "SharpLogic/LightNap";
const [OWNER, REPO] = GITHUB_REPOSITORY.split("/");
const BASE_BRANCH = process.env.BASE_BRANCH || "main";
const COMMIT_SHA = process.env.GITHUB_SHA;

// Initialize clients
const octokit = new Octokit({ auth: GITHUB_TOKEN });
let openai, anthropic;

if (AI_PROVIDER === "openai") {
  if (!OPENAI_API_KEY) {
    throw new Error("OPENAI_API_KEY is required when using OpenAI provider");
  }
  openai = new OpenAI({ apiKey: OPENAI_API_KEY });
} else if (AI_PROVIDER === "anthropic") {
  if (!ANTHROPIC_API_KEY) {
    throw new Error(
      "ANTHROPIC_API_KEY is required when using Anthropic provider"
    );
  }
  anthropic = new Anthropic({ apiKey: ANTHROPIC_API_KEY });
}

/**
 * Main execution function
 */
async function main() {
  try {
    console.log("🤖 LightNap Documentation Agent starting...");
    console.log(`📦 Repository: ${OWNER}/${REPO}`);
    console.log(`🌿 Base branch: ${BASE_BRANCH}`);
    console.log(`📝 Commit: ${COMMIT_SHA}`);
    console.log(`🤖 AI Provider: ${AI_PROVIDER}`);

    // Step 1: Get the recent changes
    const changes = await getRecentChanges();

    if (changes.length === 0) {
      console.log(
        "✅ No code changes detected that require documentation updates."
      );
      return;
    }

    console.log(`📊 Analyzing ${changes.length} changed files...`);

    // Step 2: Read existing documentation structure
    const docsStructure = await getDocsStructure();
    console.log(`📚 Found ${docsStructure.length} documentation files`);

    // Step 3: Analyze changes and determine documentation updates needed
    const docUpdates = await analyzeChangesAndProposeUpdates(
      changes,
      docsStructure
    );

    if (!docUpdates || docUpdates.updates.length === 0) {
      console.log("✅ No documentation updates needed based on code changes.");
      return;
    }

    console.log(
      `📝 Proposing ${docUpdates.updates.length} documentation updates...`
    );

    // Step 4: Create a new branch and apply changes
    const branchName = await createBranchWithUpdates(docUpdates);

    // Step 5: Create a pull request
    await createPullRequest(branchName, docUpdates);

    console.log("✨ Documentation agent completed successfully!");
  } catch (error) {
    console.error("❌ Error in documentation agent:", error);
    process.exit(1);
  }
}

/**
 * Get recent changes from the last commit
 */
async function getRecentChanges() {
  const { data: commit } = await octokit.repos.getCommit({
    owner: OWNER,
    repo: REPO,
    ref: COMMIT_SHA,
  });

  // Filter to only include source code files, exclude docs
  const codeFiles = commit.files.filter((file) => {
    const path = file.filename;
    return (
      (path.startsWith("src/") || path.startsWith("Scaffolding/")) &&
      !path.includes("/bin/") &&
      !path.includes("/obj/") &&
      !path.includes("node_modules") &&
      !path.startsWith("docs/")
    );
  });

  // Get file contents for changed files
  const changes = [];
  for (const file of codeFiles) {
    try {
      let content = "";
      if (file.status !== "removed") {
        const { data } = await octokit.repos.getContent({
          owner: OWNER,
          repo: REPO,
          path: file.filename,
          ref: COMMIT_SHA,
        });
        content = Buffer.from(data.content, "base64").toString("utf-8");
      }

      changes.push({
        filename: file.filename,
        status: file.status,
        additions: file.additions,
        deletions: file.deletions,
        patch: file.patch,
        content: content,
      });
    } catch (error) {
      console.warn(
        `⚠️  Could not fetch content for ${file.filename}:`,
        error.message
      );
    }
  }

  return changes;
}

/**
 * Get the structure of the docs folder
 */
async function getDocsStructure() {
  const docs = [];

  async function readDir(dirPath, relativePath = "") {
    const { data: contents } = await octokit.repos.getContent({
      owner: OWNER,
      repo: REPO,
      path: dirPath,
      ref: BASE_BRANCH,
    });

    for (const item of contents) {
      if (item.type === "file" && item.name.endsWith(".md")) {
        try {
          const { data } = await octokit.repos.getContent({
            owner: OWNER,
            repo: REPO,
            path: item.path,
            ref: BASE_BRANCH,
          });
          const content = Buffer.from(data.content, "base64").toString("utf-8");

          docs.push({
            path: item.path,
            name: item.name,
            content: content,
            sha: item.sha,
          });
        } catch (error) {
          console.warn(`⚠️  Could not read ${item.path}:`, error.message);
        }
      } else if (item.type === "dir" && !item.name.startsWith("_")) {
        await readDir(item.path, path.join(relativePath, item.name));
      }
    }
  }

  await readDir("docs");
  return docs;
}

/**
 * Use AI to analyze changes and propose documentation updates
 */
async function analyzeChangesAndProposeUpdates(changes, docsStructure) {
  const systemPrompt = `You are an expert technical documentation analyst for the LightNap starter kit.

LightNap is an ASP.NET Core + Angular starter kit with the following architecture:
- Backend: .NET 9, Entity Framework Core, ASP.NET Identity
- Frontend: Angular 20, PrimeNG components
- Features: User authentication, roles/claims, notifications, user settings, profile management

The documentation is organized into:
1. **getting-started/**: Setup and configuration guides
2. **concepts/**: Architecture and design patterns
3. **common-scenarios/**: How-to guides for extending the application

Your task is to analyze code changes and determine what documentation updates are needed. Focus on:
- New extensibility patterns (like notifications, user settings)
- New features that developers might want to extend
- Changes to existing patterns that affect documentation
- Breaking changes that need to be documented
- New common scenarios that should be documented

For each change, determine:
1. Whether documentation updates are needed (be conservative - not every code change needs doc updates)
2. Which documentation file(s) should be updated
3. The specific changes needed (additions, modifications, or new files)

Return your analysis as a JSON object with this structure:
{
  "summary": "Brief summary of what changed and why docs need updating",
  "updates": [
    {
      "file": "docs/path/to/file.md",
      "action": "edit|create|delete",
      "reason": "Why this update is needed",
      "changes": "Specific markdown content changes or new content"
    }
  ]
}

If no documentation updates are needed, return: { "summary": "No documentation updates needed", "updates": [] }`;

  const changesDescription = changes
    .map((change) => {
      return `
File: ${change.filename}
Status: ${change.status}
Changes: +${change.additions} -${change.deletions}

${change.patch ? `Diff:\n${change.patch}\n` : ""}
${
  change.content
    ? `Current Content:\n${change.content.substring(0, 2000)}${
        change.content.length > 2000 ? "...[truncated]" : ""
      }`
    : ""
}
---
`;
    })
    .join("\n");

  const docsDescription = docsStructure
    .map((doc) => {
      return `File: ${doc.path}\nPreview: ${doc.content.substring(
        0,
        300
      )}...\n---`;
    })
    .join("\n");

  const userPrompt = `Analyze these code changes and propose documentation updates:

## Code Changes:
${changesDescription}

## Existing Documentation Structure:
${docsDescription}

Provide your analysis as JSON.`;

  console.log("🤔 Consulting AI for documentation analysis...");

  let analysis;

  if (AI_PROVIDER === "openai") {
    const response = await openai.chat.completions.create({
      model: "gpt-4o",
      messages: [
        { role: "system", content: systemPrompt },
        { role: "user", content: userPrompt },
      ],
      temperature: 0.2,
      response_format: { type: "json_object" },
    });
    analysis = JSON.parse(response.choices[0].message.content);
  } else if (AI_PROVIDER === "anthropic") {
    const response = await anthropic.messages.create({
      model: "claude-sonnet-4-5",
      max_tokens: 4096,
      temperature: 0.2,
      system: systemPrompt,
      messages: [{ role: "user", content: userPrompt }],
    });

    // Extract JSON from Claude's response
    const content = response.content[0].text;
    // Claude might wrap JSON in markdown code blocks
    const jsonMatch =
      content.match(/```json\n([\s\S]*?)\n```/) || content.match(/\{[\s\S]*\}/);
    analysis = JSON.parse(jsonMatch ? jsonMatch[1] || jsonMatch[0] : content);
  }

  console.log(`💡 Analysis: ${analysis.summary}`);

  return analysis;
}

/**
 * Create a new branch with the proposed documentation updates
 */
async function createBranchWithUpdates(docUpdates) {
  const timestamp = new Date()
    .toISOString()
    .replace(/[:.]/g, "-")
    .substring(0, 19);
  const branchName = `docs/auto-update-${timestamp}`;

  console.log(`🌿 Creating branch: ${branchName}`);

  // Get the base branch reference
  const { data: ref } = await octokit.git.getRef({
    owner: OWNER,
    repo: REPO,
    ref: `heads/${BASE_BRANCH}`,
  });

  // Create new branch
  await octokit.git.createRef({
    owner: OWNER,
    repo: REPO,
    ref: `refs/heads/${branchName}`,
    sha: ref.object.sha,
  });

  // Apply each update
  for (const update of docUpdates.updates) {
    console.log(`📝 ${update.action}: ${update.file}`);

    if (update.action === "create") {
      await createFile(branchName, update.file, update.changes);
    } else if (update.action === "edit") {
      await updateFile(branchName, update.file, update.changes);
    } else if (update.action === "delete") {
      await deleteFile(branchName, update.file);
    }
  }

  return branchName;
}

/**
 * Create a new file in the repository
 */
async function createFile(branch, filePath, content) {
  await octokit.repos.createOrUpdateFileContents({
    owner: OWNER,
    repo: REPO,
    path: filePath,
    message: `docs: Add ${path.basename(filePath)}`,
    content: Buffer.from(content).toString("base64"),
    branch: branch,
  });
}

/**
 * Update an existing file in the repository
 */
async function updateFile(branch, filePath, newContent) {
  try {
    // Get current file to get its SHA
    const { data: currentFile } = await octokit.repos.getContent({
      owner: OWNER,
      repo: REPO,
      path: filePath,
      ref: branch,
    });

    await octokit.repos.createOrUpdateFileContents({
      owner: OWNER,
      repo: REPO,
      path: filePath,
      message: `docs: Update ${path.basename(filePath)}`,
      content: Buffer.from(newContent).toString("base64"),
      branch: branch,
      sha: currentFile.sha,
    });
  } catch (error) {
    console.warn(`⚠️  Could not update ${filePath}:`, error.message);
  }
}

/**
 * Delete a file from the repository
 */
async function deleteFile(branch, filePath) {
  try {
    const { data: currentFile } = await octokit.repos.getContent({
      owner: OWNER,
      repo: REPO,
      path: filePath,
      ref: branch,
    });

    await octokit.repos.deleteFile({
      owner: OWNER,
      repo: REPO,
      path: filePath,
      message: `docs: Remove ${path.basename(filePath)}`,
      branch: branch,
      sha: currentFile.sha,
    });
  } catch (error) {
    console.warn(`⚠️  Could not delete ${filePath}:`, error.message);
  }
}

/**
 * Create a pull request with the documentation updates
 */
async function createPullRequest(branchName, docUpdates) {
  console.log("📬 Creating pull request...");

  const prBody = `## 🤖 Automated Documentation Update

This PR was automatically generated by the LightNap Documentation Agent after analyzing recent code changes.

### Summary
${docUpdates.summary}

### Proposed Changes
${docUpdates.updates
  .map((u) => `- **${u.action}** \`${u.file}\`: ${u.reason}`)
  .join("\n")}

### Instructions
Please review these changes carefully:
1. Verify that the documentation accurately reflects the code changes
2. Check for completeness and clarity
3. Edit the documentation directly in this PR if needed
4. Merge when satisfied, or close if not needed

---
*This PR was created automatically. The agent analyzes code changes but human review is required.*`;

  const { data: pr } = await octokit.pulls.create({
    owner: OWNER,
    repo: REPO,
    title: "📚 Automated documentation update",
    head: branchName,
    base: BASE_BRANCH,
    body: prBody,
  });

  console.log(`✅ Pull request created: ${pr.html_url}`);

  // Add labels
  try {
    await octokit.issues.addLabels({
      owner: OWNER,
      repo: REPO,
      issue_number: pr.number,
      labels: ["documentation", "automated"],
    });
  } catch (error) {
    console.warn("⚠️  Could not add labels:", error.message);
  }

  return pr;
}

// Run the main function
main();
