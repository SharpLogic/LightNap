module.exports = {
    "backend-api": {
        input: {
            target: "https://localhost:7266/swagger/v1/swagger.json",
            override: {
                transformer: "./scripts/orval.unwrapper.js",
            },
        },
        output: {
            mode: "single",
            clean: true,
            target: "src/app/core/backend-api/services/lightnap-api.ts",
            schemas: "src/app/core/backend-api/models",
            client: "angular",
            httpClient: true,
            mock: true,
            namingConvention: "kebab-case",
            override: {
                useDates: true,
            },
        },
        hooks: {
            afterAllFilesWrite: "prettier --write",
        },
    },
};
