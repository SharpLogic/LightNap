# Documentation Agent Prompts

This directory contains the AI prompts used by the documentation agent. Prompts are stored in separate files to make them easier to edit and maintain without modifying the script code.

## Prompt Files

### `analysis-system.md`

The system prompt for the initial analysis phase. This defines the AI's role as a technical documentation analyst and provides context about the LightNap project structure. It also specifies the JSON output format expected.

**Edit this file to:**

- Update the project description or architecture details
- Modify what the AI should focus on when analyzing changes
- Adjust the JSON output structure
- Change how the AI should approach documentation updates

### `analysis-user.md`

The user prompt template for the initial analysis phase. Contains placeholders that are replaced at runtime:

- `{CHANGES_DESCRIPTION}` - Replaced with details about code changes
- `{DOCS_DESCRIPTION}` - Replaced with the current documentation structure

**Edit this file to:**

- Change how code changes are presented to the AI
- Modify instructions for the analysis

### `edit-system.md`

The system prompt for the documentation editing phase. This instructs the AI to act as a technical editor and return only the complete updated file content.

**Edit this file to:**

- Adjust the AI's role during the editing phase
- Modify constraints on output format

### `edit-user.md`

The user prompt template for the editing phase. Contains placeholders:

- `{FILE_PATH}` - The path to the file being edited
- `{REASON}` - Why this file needs updating
- `{CURRENT_CONTENT}` - The complete current content of the file
- `{CHANGES_DESCRIPTION}` - Details about the code changes

**Edit this file to:**

- Change how existing file content is presented
- Modify instructions for generating updates
- Adjust the context provided during editing

## How Prompts Are Used

1. **Analysis Phase**: The agent uses `analysis-system.md` and `analysis-user.md` to identify which documentation files need updating
2. **Editing Phase**: For each file that needs editing, the agent uses `edit-system.md` and `edit-user.md` to generate the complete updated content

## Tips for Editing Prompts

- Keep instructions clear and specific
- Use concrete examples where possible
- Test changes by running the agent on sample code changes
- Remember that these prompts work with both OpenAI and Anthropic models
- Placeholders in curly braces `{LIKE_THIS}` are replaced at runtime by the script
