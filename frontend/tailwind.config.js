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
                primaryBlue: "var(--primary-blue)",
                primaryDark: "var(--primary-dark)",
                primaryLight: "var(--primary-light)",

                secondarySuccess: "var(--secondary-success",
                secondarywarning: "var(--secondary-warning",
                secondaryError: "var(--secondary-error",
                secondaryInfo: "var(--secondary-info",




            }
        },
    },
    plugins: [],
}
