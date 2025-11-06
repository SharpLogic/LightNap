# Using OpenAI GPT-4 with the Documentation Agent

This guide shows you how to use OpenAI's GPT-4 for the LightNap documentation agent.

## Why Use OpenAI?

- **Default option**: Works out of the box, no provider configuration needed
- **Widely adopted**: Most developers are familiar with OpenAI's APIs
- **JSON mode**: Native support for structured JSON responses
- **Proven reliability**: Extensive production usage and documentation
- **Multiple models**: Choose between quality and cost

## Quick Setup

### 1. Get an OpenAI API Key

1. Visit [OpenAI Platform](https://platform.openai.com/)
2. Sign up or log in
3. Navigate to **API Keys** in your account settings
4. Click **Create new secret key**
5. Give it a name like "LightNap Docs Agent"
6. Copy the key (save it securely - you won't see it again)

### 2. Add to GitHub Secrets

1. Go to your repository on GitHub
2. Click **Settings** > **Secrets and variables** > **Actions**
3. Click **New repository secret**
4. Name: `OPENAI_API_KEY`
5. Value: Paste your API key
6. Click **Add secret**

### 3. Enable the Agent

1. Still in **Settings** > **Secrets and variables** > **Actions**
2. Click the **Variables** tab
3. Click **New repository variable**
4. Name: `RUN_DOCS_AGENT`
5. Value: `true`
6. Click **Add variable**

That's it! The agent will use OpenAI by default (no `AI_PROVIDER` variable needed).

## Models Available

The agent uses **GPT-4o** by default, which offers excellent quality for technical documentation.

### Change to a Different Model

Edit `.github/scripts/docs-agent/index.js` and find the OpenAI section:

```javascript
if (AI_PROVIDER === 'openai') {
  const response = await openai.chat.completions.create({
    model: 'gpt-4o',  // Change this line
    messages: [
      // ...
```

**Available models**:

- `gpt-4o` - Default, best balance of quality and speed
- `gpt-4o-mini` - Faster and much cheaper, still good quality
- `gpt-4-turbo` - Previous generation, still very capable
- `gpt-4` - Original GPT-4 (slower, more expensive)

**Recommended**: Start with `gpt-4o`, switch to `gpt-4o-mini` if you want to reduce costs.

## Cost Comparison

Approximate costs per analysis (varies by size):

| Model | Per Run | Monthly (active repo) |
|-------|---------|----------------------|
| GPT-4o | $0.01-0.10 | $3-30 |
| GPT-4o-mini | $0.001-0.01 | $0.30-3 |
| GPT-4-turbo | $0.02-0.15 | $6-45 |
| GPT-4 | $0.05-0.50 | $15-150 |

**Recommendation**: Use `gpt-4o-mini` for cost-conscious deployments. It's 10-15x cheaper than `gpt-4o` and still produces excellent documentation.

For comparison with Anthropic:

| Anthropic Model | Per Run | Monthly (active repo) |
|-----------------|---------|----------------------|
| Claude 3.5 Sonnet | $0.01-0.12 | $3-35 |
| Claude 3.5 Haiku | $0.001-0.02 | $0.30-6 |

## Switching to Anthropic

If you want to try Anthropic Claude instead:

1. Get an Anthropic API key from [console.anthropic.com](https://console.anthropic.com/)
2. Add `ANTHROPIC_API_KEY` to GitHub Secrets
3. Add `AI_PROVIDER` variable with value `anthropic`
4. The next run will use Claude

You can switch back by changing `AI_PROVIDER` to `openai` (or removing it, as OpenAI is the default).

## Testing Your Setup

After configuration:

1. Make a small change in `src/` (like adding a comment)
2. Commit and push to main
3. Go to **Actions** tab
4. Watch for the "Automated Documentation Agent" workflow
5. Check the logs - you should see "🤖 AI Provider: openai"
6. If docs need updating, review the created PR

## Troubleshooting

### Error: "OPENAI_API_KEY is required"

- Make sure you added `OPENAI_API_KEY` to GitHub Secrets (not Variables)
- Verify the secret name is exactly `OPENAI_API_KEY` (case-sensitive)
- Check that the secret is set at the repository level, not organization level (unless intended)

### "Insufficient quota" or Rate Limit Errors

- Check your OpenAI account has available credits
- Visit [platform.openai.com/account/billing](https://platform.openai.com/account/billing)
- Add payment method or purchase credits
- Check rate limits for your tier at [platform.openai.com/account/limits](https://platform.openai.com/account/limits)

### Agent Using Wrong Model

- Verify you edited the correct section in `index.js` (look for `if (AI_PROVIDER === 'openai')`)
- Commit and push your changes to the repository
- The workflow uses the code from the repository, not your local machine

### JSON Parsing Errors

OpenAI's `response_format: { type: 'json_object' }` ensures valid JSON responses. If you still see errors:

1. Check the workflow logs for the exact error
2. Verify you're using a model that supports JSON mode (GPT-4o, GPT-4-turbo, GPT-3.5-turbo)
3. Report an issue if it persists

## Optimizing Costs

### 1. Use GPT-4o-mini

The easiest way to reduce costs is to switch to `gpt-4o-mini`:

```javascript
model: 'gpt-4o-mini',  // 10-15x cheaper than gpt-4o
```

Test the quality first - it's often excellent for documentation tasks.

### 2. Reduce Frequency

Edit `.github/workflows/docs-agent.yaml` to trigger less often:

```yaml
on:
  push:
    branches:
      - main
    paths:
      - 'src/LightNap.Core/**'  # Only trigger on core changes
      # - 'src/**'  # Comment out to reduce scope
```

### 3. Batch Changes

Don't trigger on every commit:

```yaml
on:
  schedule:
    - cron: '0 0 * * 1'  # Run weekly on Monday
  workflow_dispatch:      # Keep manual trigger
```

### 4. Limit Content Size

Edit `index.js` to reduce the amount of content sent:

```javascript
// Reduce from 2000 to 1000 characters
content.substring(0, 1000)
```

## Best Practices

1. **Start with GPT-4o**: Use the default to evaluate quality
2. **Switch to mini for savings**: If quality is good, try `gpt-4o-mini` for 10x cost reduction
3. **Monitor usage**: Check [platform.openai.com/usage](https://platform.openai.com/usage) regularly
4. **Set spending limits**: Configure billing alerts in your OpenAI account
5. **Test locally**: Use the script locally with test data before production use

## Advanced Configuration

### Custom Temperature

Adjust creativity vs consistency:

```javascript
temperature: 0.5,  // Lower = more consistent (default: 0.7)
```

### Max Tokens

Limit response length (and cost):

```javascript
max_tokens: 2000,  // Reduce from default if needed
```

### System Prompt

Customize how the AI analyzes (edit in `index.js`):

```javascript
const systemPrompt = `You are an expert technical documentation analyst...
Be very conservative - only propose updates when absolutely necessary.
Focus on breaking changes and new extensibility patterns.`;
```

## API Key Security

### Security Best Practices

- ✅ Store in GitHub Secrets only
- ✅ Never commit to code or logs
- ✅ Rotate keys periodically
- ✅ Use descriptive names to track usage
- ❌ Never share keys publicly
- ❌ Don't use personal keys for shared repos

### If Key is Compromised

1. Immediately revoke the key at [platform.openai.com/api-keys](https://platform.openai.com/api-keys)
2. Create a new key
3. Update GitHub Secret
4. Review OpenAI usage logs for unauthorized access

## Need Help?

- **Main documentation**: `docs/github-actions-workflows/automated-docs-agent.md`
- **OpenAI docs**: [platform.openai.com/docs](https://platform.openai.com/docs)
- **OpenAI community**: [community.openai.com](https://community.openai.com)
- **Issues**: Open an issue with `documentation` and `automated` labels

## Summary

Using OpenAI GPT-4 with the documentation agent:

1. ✅ Get OpenAI API key from platform.openai.com
2. ✅ Add as `OPENAI_API_KEY` secret in GitHub
3. ✅ Enable with `RUN_DOCS_AGENT=true` variable
4. ✅ No `AI_PROVIDER` variable needed (OpenAI is default)
5. ✅ Test and monitor usage

**Cost Optimization Tips**:

- Use `gpt-4o-mini` for 10x cost savings
- Trigger less frequently
- Monitor usage dashboard

GPT-4o provides excellent results for technical documentation and is the proven, default choice for the LightNap documentation agent!
