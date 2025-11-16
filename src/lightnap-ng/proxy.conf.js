const PROXY_CONFIG = {
  "/api": {
    target: process.env.PROXY_TARGET || "https://localhost:7266",
    secure: false,
    changeOrigin: true,
  }
};

module.exports = PROXY_CONFIG;
