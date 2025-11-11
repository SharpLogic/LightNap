// Note: This file uses a name other than "tailwind.config.js" so that both the build and tests will run without other workarounds.
// See https://github.com/SharpLogic/LightNap/issues/21.

import tailwindcssPrimeui from "tailwindcss-primeui";

export default {
    darkMode: ["selector", '[class="app-dark"]'],
    content: ["./src/**/*.{html,ts,css}", "./index.html"],
    plugins: [tailwindcssPrimeui],
    theme: {
        screens: {
            sm: "576px",
            md: "768px",
            lg: "992px",
            xl: "1200px",
            "2xl": "1920px",
        },
    },
};
