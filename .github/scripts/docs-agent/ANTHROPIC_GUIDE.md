# Using Anthropic Claude with the Documentation Agent

This guide shows you how to use Anthropic's Claude instead of OpenAI's GPT-4 for the LightNap documentation agent.

## Why Use Anthropic?

- **Excellent for technical writing**: Claude is known for strong documentation capabilities
- **Competitive pricing**: Often comparable or cheaper than GPT-4
- **Different perspective**: May provide alternative analysis approaches
- **Fast models available**: Claude 3.5 Haiku is very fast and cost-effective

## Quick Setup

### 1. Get an Anthropic API Key

1. Visit [Anthropic Console](https://console.anthropic.com/)
2. Sign up or log in
3. Navigate to **API Keys**
4. Click **Create Key**
5. Give it a name like "LightNap Docs Agent"
6. Copy the key (save it securely)

### 2. Add to GitHub Secrets

1. Go to your repository on GitHub
2. Click **Settings** > **Secrets and variables** > **Actions**
3. Click **New repository secret**
4. Name: `ANTHROPIC_API_KEY`
5. Value: Paste your API key
6. Click **Add secret**

### 3. Set AI Provider

1. Still in **Settings** > **Secrets and variables** > **Actions**
2. Click the **Variables** tab
3. Click **New repository variable**
4. Name: `AI_PROVIDER`
5. Value: `anthropic`
6. Click **Add variable**

### 4. Enable the Agent (if not already)

1. Click **New repository variable**
2. Name: `RUN_DOCS_AGENT`
3. Value: `true`
4. Click **Add variable**

## Models Available

The agent uses **Claude 3.5 Sonnet** by default, which offers excellent quality for technical documentation.

### Change to a Different Model

Edit `.github/scripts/docs-agent/index.js` and find the Anthropic section:

```javascript
} else if (AI_PROVIDER === 'anthropic') {
  const response = await anthropic.messages.create({
    model: 'claude-3-5-sonnet-20241022',  // Change this line
    max_tokens: 4096,
    // ...
```

**Available models**:

- `claude-3-5-sonnet-20241022` - Default, best quality for technical writing
- `claude-3-5-haiku-20241022` - Faster and cheaper, still good quality
- `claude-3-opus-20240229` - Highest capability (most expensive)

## Cost Comparison

Approximate costs per analysis (varies by size):

| Model | Per Run | Monthly (active repo) |
|-------|---------|----------------------|
| Claude 3.5 Sonnet | $0.01-0.12 | $3-35 |
| Claude 3.5 Haiku | $0.001-0.02 | $0.30-6 |
| Claude 3 Opus | $0.05-0.50 | $15-150 |

For comparison:

| OpenAI Model | Per Run | Monthly (active repo) |
|--------------|---------|----------------------|
| GPT-4o | $0.01-0.10 | $3-30 |
| GPT-4o-mini | $0.001-0.01 | $0.30-3 |

## Switching Between Providers

You can easily switch between OpenAI and Anthropic:

1. Make sure both API keys are in GitHub Secrets
2. Change the `AI_PROVIDER` variable to `openai` or `anthropic`
3. The next workflow run will use the selected provider

No code changes needed!

## Testing Your Setup

After configuration:

1. Make a small change in `src/` (like adding a comment)
2. Commit and push to main
3. Go to **Actions** tab
4. Watch for the "Automated Documentation Agent" workflow
5. Check the logs - you should see "🤖 AI Provider: anthropic"
6. If docs need updating, review the created PR

## Troubleshooting

### Error: "ANTHROPIC_API_KEY is required"

- Make sure you added `ANTHROPIC_API_KEY` to GitHub Secrets (not Variables)
- Make sure `AI_PROVIDER` is set to `anthropic`

### Agent Still Using OpenAI

- Check that `AI_PROVIDER` variable is set to `anthropic` (not OpenAI)
- Verify the variable is in the repository settings, not secrets

### Poor Results with Claude

- Try Claude 3.5 Sonnet instead of Haiku for better quality
- Adjust the system prompt in `index.js` if needed
- Claude may format output differently - the agent handles this automatically

### JSON Parsing Errors

The agent automatically handles Claude's tendency to wrap JSON in markdown code blocks. If you still see errors:

1. Check the workflow logs for the exact error
2. Claude's response should be automatically extracted
3. Report an issue if it persists

## Best Practices

1. **Start with Sonnet**: Use Claude 3.5 Sonnet first to evaluate quality
2. **Try Haiku for cost savings**: If Sonnet works well, test Haiku for lower costs
3. **Compare with OpenAI**: Run a few analyses with both providers to see which you prefer
4. **Monitor usage**: Check your Anthropic dashboard for API usage and costs

## Need Help?

- See the main documentation: `docs/github-actions-workflows/automated-docs-agent.md`
- Check Anthropic's [API documentation](https://docs.anthropic.com/)
- Open an issue with the `documentation` and `automated` labels

## Summary

Using Anthropic Claude with the documentation agent is straightforward:

1. ✅ Get Anthropic API key
2. ✅ Add as `ANTHROPIC_API_KEY` secret
3. ✅ Set `AI_PROVIDER=anthropic` variable
4. ✅ Enable with `RUN_DOCS_AGENT=true`
5. ✅ Test and monitor

Claude often excels at technical documentation tasks and may provide excellent results for your LightNap documentation needs!
