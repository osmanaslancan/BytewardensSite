/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./Views/**/*.{html,js,ts,cshtml}"],
    blocklist: [
        'container',
        'collapse',
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}

