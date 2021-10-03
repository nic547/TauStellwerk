// This contains various functions to access js specific functionality from C#/WASM
// noinspection JSUnusedGlobalSymbols

function setTheme(theme) {
    const element = document.documentElement;
    element.setAttribute("data-theme", theme);
}

function setItem(key, value) {
    localStorage.setItem(key, value)
}

function getItem(key){
    return localStorage.getItem(key);
}