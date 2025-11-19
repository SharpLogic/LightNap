const PROXY_CONFIG = {
    "/api": {
        target: "https://localhost:7266",
        secure: false,
        changeOrigin: true,
        ws: true,
        logLevel: "debug",
    },
};

module.exports = PROXY_CONFIG;
