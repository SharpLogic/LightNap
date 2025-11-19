const PROXY_CONFIG = {
  "/api": {
    target: "https://lightnap.azurewebsites.net",
    secure: true,
    changeOrigin: true,
    ws: true,
  }
};

module.exports = PROXY_CONFIG;
