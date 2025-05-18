// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const applyTheme = (theme) => {
    document.body.classList.remove('light-mode', 'dark-mode');
    document.body.classList.add(`${theme}-mode`);
};

const savedTheme = localStorage.getItem('theme') || 'light';
applyTheme(savedTheme);

document.getElementById('themeToggleBtn').addEventListener('click', () => {
    const newTheme = document.body.classList.contains('light-mode') ? 'dark' : 'light';
    localStorage.setItem('theme', newTheme);
    applyTheme(newTheme);
});