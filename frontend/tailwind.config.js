/* eslint-env node */
/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    blue: "rgb(var(--primary-blue) / <alpha-value>)",
                    dark: "rgb(var(--primary-dark) / <alpha-value>)",
                    light: "rgb(var(--primary-light) / <alpha-value>)",
                },
                secondary: {
                    success: "rgb(var(--secondary-success) / <alpha-value>)",
                    warning: "rgb(var(--secondary-warning) / <alpha-value>)",
                    error: "rgb(var(--secondary-error) / <alpha-value>)",
                    info: "rgb(var(--secondary-info) / <alpha-value>)",
                },
            }




        }
    },

    plugins: [],
}
