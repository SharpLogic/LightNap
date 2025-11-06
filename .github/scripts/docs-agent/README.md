# LightNap Documentation Agent

An AI-powered GitHub Actions workflow that automatically analyzes code changes and proposes documentation updates.

## Overview

This agent helps keep LightNap documentation synchronized with code changes by:

- **Analyzing** code changes from merges to main
- **Understanding** LightNap's architecture and extensibility patterns
- **Proposing** specific documentation updates (adds, edits, deletions)
- **Creating** pull requests with suggested changes for human review

## Quick Start

### Prerequisites

**Choose Your AI Provider**:

- **OpenAI** (GPT-4) - Default, widely used
- **Anthropic** (Claude) - Excellent for technical writing

You'll also need:

- GitHub repository with write permissions

### Configuration

#### Option 1: Using OpenAI (Default)

1. **Add OpenAI API Key**:
   - Go to repository **Settings** > **Secrets and variables** > **Actions**
   - Add secret: `OPENAI_API_KEY` = your OpenAI API key

2. **Enable the Workflow**:
   - Go to **Settings** > **Secrets and variables** > **Actions** > **Variables**
   - Add variable: `RUN_DOCS_AGENT` = `true`

#### Option 2: Using Anthropic Claude

1. **Add Anthropic API Key**:
   - Go to repository **Settings** > **Secrets and variables** > **Actions**
   - Add secret: `ANTHROPIC_API_KEY` = your Anthropic API key

2. **Set AI Provider**:
   - Go to **Settings** > **Secrets and variables** > **Actions** > **Variables**
   - Add variable: `AI_PROVIDER` = `anthropic`

3. **Enable the Workflow**:
   - Add variable: `RUN_DOCS_AGENT` = `true`

#### Testing

- Merge a change to `main` that modifies files in `src/`
- Check the **Actions** tab for workflow execution
- Review any auto-generated documentation PRs

## How It Works

### Workflow Trigger

The agent runs automatically when:

- Code is merged to the `main` branch
- Changes are in `src/` or `Scaffolding/` folders

### Analysis Process

1. **Fetch Changes**: Retrieves the most recent commit's changes
2. **Read Documentation**: Loads existing documentation structure
3. **AI Analysis**: Uses AI (GPT-4 or Claude) to understand what docs need updating
4. **Create Branch**: Creates a new branch with proposed changes
5. **Create PR**: Opens a pull request for human review

### AI Understanding

The agent uses a specialized prompt that understands:

- LightNap's ASP.NET Core + Angular architecture
- Documentation structure (concepts, getting-started, common-scenarios)
- Extensibility patterns (notifications, user settings, etc.)
- When documentation updates are actually needed (conservative approach)

## Usage

### Automatic Operation

Once enabled, the agent works automatically:

```graph
Developer → Merge PR to main
         ↓
Agent → Analyze changes
      ↓
      Create documentation PR
      ↓
Human → Review and merge
```

### Manual Trigger

You can also run the agent manually:

1. Go to **Actions** tab
2. Select **Automated Documentation Agent**
3. Click **Run workflow**
4. Select branch and run

## Customization

### Adjust AI Behavior

Edit `index.js` to customize the system prompt:

```javascript
const systemPrompt = `You are an expert technical documentation analyst...`;
```

Consider adjusting for:

- New documentation sections
- Different style guidelines
- Modified sensitivity threshold

### Change Trigger Conditions

Edit `.github/workflows/docs-agent.yaml`:

```yaml
on:
  push:
    branches:
      - main
    paths:
      - 'src/**'           # Add or remove paths
      - 'Scaffolding/**'
```

### Switch AI Providers

Change the `AI_PROVIDER` variable in GitHub repository settings:

- Set to `openai` for OpenAI GPT-4
- Set to `anthropic` for Anthropic Claude

### Use Different AI Model

**For OpenAI** - Edit `index.js`:

```javascript
const response = await openai.chat.completions.create({
  model: 'gpt-4o-mini',  // Cheaper option
  // ...
});
```

**For Anthropic** - Edit `index.js`:

```javascript
const response = await anthropic.messages.create({
  model: 'claude-3-5-haiku-20241022',  // Faster, cheaper option
  // ...
});
```

## Cost Estimation

**OpenAI**:

- GPT-4o: $0.01-0.10 per run
- GPT-4o-mini: $0.001-0.01 per run

**Anthropic**:

- Claude 3.5 Sonnet: $0.01-0.12 per run
- Claude 3.5 Haiku: $0.001-0.02 per run

**Monthly**: $3-35 for moderately active repository

## Troubleshooting

### Agent Not Running

**Check configuration**:

```bash
# Verify RUN_DOCS_AGENT is set to 'true'
# Verify API key secret exists (OPENAI_API_KEY or ANTHROPIC_API_KEY)
# Verify AI_PROVIDER is set correctly if using Anthropic
# Check that changes are in monitored paths
```

### Poor Quality Suggestions

**Adjust the prompt**:

- Edit the `systemPrompt` in `index.js`
- Add examples of good documentation
- Be more specific about requirements

### High API Costs

**Reduce frequency**:

- Run less often
- Use cheaper models (gpt-4o-mini or claude-3-5-haiku)
- Switch providers to compare costs
- Increase minimum change threshold

## Security

- ✅ API key stored in GitHub Secrets
- ✅ Workflow scoped permissions
- ✅ Human review required before merging
- ⚠️  Always review automated content
- ⚠️  Monitor API usage and costs

## Limitations

**What it CAN do**:

- Detect new extensibility patterns
- Propose specific documentation changes
- Understand LightNap architecture
- Create well-formatted markdown

**What it CANNOT do**:

- Replace human technical writing
- Understand business domain context
- Guarantee perfect accuracy
- Test documentation examples

## Documentation

For complete documentation, see the automated docs agent guide in the main documentation site.

## Support

- Open issues with `documentation` and `automated` labels
- Check GitHub Actions logs for errors
- Review the workflow YAML and agent script

## License

Same as LightNap - see LICENSE file in repository root.
