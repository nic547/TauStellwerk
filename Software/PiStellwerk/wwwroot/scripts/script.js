"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.init = exports.commandButtonPressed = void 0;
var username;
var isRunning = false;
var isBlocked;
var statusIntervalId;
async function commandButtonPressed() {
    if (isBlocked) {
        return;
    }
    postStatusChange(!isRunning);
    handleStatusChange(!isRunning, `${username} (You)`);
    isRunning = (!isRunning);
}
exports.commandButtonPressed = commandButtonPressed;
function handleStatusChange(isRunning, username) {
    let div = document.getElementById("CommandButton");
    let title = document.getElementById("CommandTitle");
    let details = document.getElementById("CommandDetails");
    if (isRunning) {
        div.classList.remove("StoppedButton");
        div.classList.add("RunningButton");
        title.innerHTML = "RUNNING";
        details.innerHTML = `PiStellwerk started by ${username} `;
    }
    else {
        isBlocked = true;
        div.classList.remove("RunningButton");
        div.classList.add("StoppedBlockingButton");
        title.innerHTML = "STOPPED (LOCKED)";
        details.innerHTML = `PiStellwerk stopped by ${username} `;
        setTimeout(function () {
            isBlocked = false;
            div.classList.remove("StoppedBlockingButton");
            title.innerHTML = "STOPPED";
            div.classList.add("StoppedButton");
        }, 2500);
    }
}
async function postStatusChange(isRunning) {
    let bodyObject = { isRunning: isRunning, lastActionUsername: username };
    let bodyContent = JSON.stringify(bodyObject);
    console.log(bodyContent);
    fetch("/status", {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: bodyContent
    });
}
function init() {
    username = Math.floor(Math.random() * 10000000).toString();
    updateDisplayedUsername();
    statusIntervalId = setInterval(() => regularUpdate(), 500);
    document.getElementById("CommandButton").addEventListener("click", commandButtonPressed);
    document.getElementById("username").addEventListener("click", promttForUsername);
}
exports.init = init;
function promttForUsername() {
    let newUsername;
    while (true) {
        newUsername = window.prompt("Enter your Username");
        if (newUsername !== "") {
            break;
        }
    }
    clearInterval(statusIntervalId);
    fetch("/user", {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify([{ name: username, UserAgent: navigator.userAgent }, { name: newUsername, UserAgent: navigator.userAgent }])
    }).then(() => {
        username = newUsername;
        updateDisplayedUsername();
        statusIntervalId = setInterval(() => regularUpdate(), 500);
    });
}
function updateDisplayedUsername() {
    document.getElementById("username").innerHTML = `User: ${username}`;
}
function regularUpdate() {
    let bodyContent = JSON.stringify({ name: username, UserAgent: navigator.userAgent });
    fetch("/status", {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        },
        body: bodyContent
    }).then((response) => {
        return response.json();
    }).then((data) => {
        if (data.isRunning != isRunning) {
            isRunning = data.isRunning;
            handleStatusChange(isRunning, data.lastActionUsername);
        }
    });
}
//# sourceMappingURL=script.js.map