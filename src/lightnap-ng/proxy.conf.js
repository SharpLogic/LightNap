const PROXY_CONFIG = {
  "/api": {
    target: "https://localhost:7266",
    secure: false,
    changeOrigin: true,
  }
};

module.exports = PROXY_CONFIG;
