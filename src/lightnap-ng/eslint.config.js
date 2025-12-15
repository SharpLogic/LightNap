const typescriptEslintPlugin = require("@typescript-eslint/eslint-plugin");
const typescriptEslintParser = require("@typescript-eslint/parser");
const angularEslintPlugin = require("@angular-eslint/eslint-plugin");
const angularTemplateParser = require("@angular-eslint/template-parser");
const preferArrow = require("eslint-plugin-prefer-arrow");
const prettier = require("eslint-plugin-prettier");

module.exports = [
    {
        ignores: [
            "node_modules/**",
            "dist/**",
            "build/**",
            "coverage/**",
            ".nyc_output/**",
            "cypress/**",
            "/tmp/**",
            ".angular/**",
            ".vscode/**",
            ".idea/**",
            "projects/**/*",
            "src/environments/environment.*.ts",
            "*.spec.ts",
            "src/main.ts",
        ],
    },
    {
        files: ["**/*.ts"],
        languageOptions: {
            parser: typescriptEslintParser,
            parserOptions: {
                project: (filePath) => {
                    if (filePath.includes(".spec.ts")) {
                        return "./tsconfig.spec.json";
                    }
                    return "./tsconfig.json";
                },
                createDefaultProgram: true,
                sourceType: "module",
            },
        },
        plugins: {
            "@typescript-eslint": typescriptEslintPlugin,
            "@angular-eslint": angularEslintPlugin,
            "prefer-arrow": preferArrow,
            prettier: prettier,
        },
        rules: {
            ...typescriptEslintPlugin.configs.recommended.rules,
            ...angularEslintPlugin.configs.recommended.rules,
            "@angular-eslint/directive-selector": [
                "error",
                {
                    type: "attribute",
                    prefix: ["app", "ln"],
                    style: "camelCase",
                },
            ],
            "@angular-eslint/component-selector": [
                "error",
                {
                    type: "element",
                    prefix: ["app", "ln"],
                    style: "kebab-case",
                },
            ],
            "@typescript-eslint/explicit-function-return-types": [
                "warn",
                {
                    allowExpressions: true,
                },
            ],
            "@typescript-eslint/no-explicit-any": "warn",
            "prefer-arrow/prefer-arrow-functions": [
                "warn",
                {
                    disallowPrototype: true,
                    singleReturnOnly: false,
                    classPropertiesAllowed: false,
                },
            ],
            "no-unused-vars": "off",
            "@typescript-eslint/no-unused-vars": [
                "error",
                {
                    argsIgnorePattern: "^_",
                    varsIgnorePattern: "^_",
                    caughtErrorsIgnorePattern: "^_",
                },
            ],
            "prettier/prettier": [
                "error",
                {
                    endofLine: "auto",
                },
            ],
            "linebreak-style": ["error", "windows"],
        },
    },
    {
        files: ["**/*.html"],
        languageOptions: {
            parser: angularTemplateParser,
        },
        plugins: {
            "@angular-eslint": angularEslintPlugin,
            prettier: prettier,
        },
        rules: {
            ...angularEslintPlugin.configs.recommended.rules,
            "prettier/prettier": "error",
        },
    },
];
