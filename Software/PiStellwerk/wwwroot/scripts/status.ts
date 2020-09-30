import * as User from "./user.js"

let isRunning = false;
let isBlocked: boolean;
let statusIntervalHandle: number;

const commandButton = document.getElementById("CommandButton") as HTMLDivElement;
const commandTitle = document.getElementById("CommandTitle");
const commandDetails = document.getElementById("CommandDetails");


document.addEventListener("DOMContentLoaded", () => {
    console.log("Status module initialization started");
    commandButton.addEventListener("click", commandButtonPressed);

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

    if (isRunning) {
        commandButton.classList.remove("StoppedButton");
        commandButton.classList.add("RunningButton");
        commandTitle.innerHTML = "RUNNING";
        commandDetails.innerHTML = `PiStellwerk started by ${username} `;
    }
    else {
        isBlocked = true;
        commandButton.classList.remove("RunningButton");
        commandButton.classList.add("StoppedBlockingButton");
        commandTitle.innerHTML = "STOPPED (LOCKED)";
        commandDetails.innerHTML = `PiStellwerk stopped by ${username} `;

        setTimeout(() => {
            isBlocked = false;
            commandButton.classList.remove("StoppedBlockingButton");
            commandTitle.innerHTML = "STOPPED";
            commandButton.classList.add("StoppedButton");
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