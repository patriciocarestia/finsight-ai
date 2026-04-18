/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        gain: "#4ade80",
        loss: "#f87171",
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
        display: ['"Space Grotesk"', "Inter", "sans-serif"],
      },
    },
  },
  plugins: [],
};
