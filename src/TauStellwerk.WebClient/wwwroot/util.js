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

function isDarkModePreferred(){
    let value = window.matchMedia("(prefers-color-scheme:dark)").matches;
    console.log(value);
    return value;
}

function postImageFormData(url) {
    const form = document.getElementById("imageForm");
    const imageInput = form.elements[0];

    if (!imageInput.value) {
        return;
    }

    const xhr = new XMLHttpRequest();
    const fd = new FormData(form);

    xhr.open("POST", url);
    xhr.send(fd);
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

function resetEngineEditModalScroll() {
    const modal = document.getElementById("EngineEditModalContent");
    modal.scrollTop = 0;
}
