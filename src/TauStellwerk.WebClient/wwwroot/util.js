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

function isWakeLockSupported() {
    return "wakeLock" in navigator;
}

let wakeLock = null;

async function enableWakeLock() {
    if (wakeLock === null) {
        wakeLock = await navigator.wakeLock.request();
        document.addEventListener("visibilitychange", reenableWakeLock);
    }
}
function disableWakeLock() {
    if (wakeLock !== null) {
        wakeLock.release();
        wakeLock = null;
        document.removeEventListener("visibilitychange", reenableWakeLock);
    }
}

async function reenableWakeLock() {
    if (document.visibilityState === "visible") {
        wakeLock = await navigator.wakeLock.request();
    }
}
