var isRunning: boolean = false;
var isBlocked: boolean;
var statusIntervalHandle: number;

import * as User from "./user.js"


document.addEventListener("DOMContentLoaded", () => {
    console.log("Status module initialization started");
    document.getElementById("CommandButton").addEventListener("click", commandButtonPressed);

    startStatusUpdates();

    console.log("Status module initialization was completed");
});

async function commandButtonPressed() {
    if (isBlocked) { return;}
    postStatusChange(!isRunning);
    handleStatusChange(!isRunning, `${User.getUsername()} (You)`);
    isRunning = (!isRunning);
}

export function startStatusUpdates() {
    statusIntervalHandle = setInterval(() => regularUpdate(), 500);
}

export function stopStatusUpdates() {
    clearInterval(statusIntervalHandle);
}

function handleStatusChange(isRunning: boolean, username: string) {
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

async function postStatusChange(isRunning: boolean) {
    let bodyObject = { isRunning: isRunning, lastActionUsername: User.getUsername() }
    let bodyContent = JSON.stringify(bodyObject);
    console.log(bodyContent);
    fetch("/status",
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: bodyContent
        }
    );
}

function regularUpdate() {
    let bodyContent = JSON.stringify({ name: User.getUsername(), UserAgent: navigator.userAgent });
    fetch("/status",
        {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json'
            },
            body: bodyContent
        }
            
    ).then((response) => {
        return response.json();
    }).then((data) => {
        if (data.isRunning != isRunning) {
            isRunning = data.isRunning;
            handleStatusChange(isRunning, data.lastActionUsername);
        }
        });
    
}