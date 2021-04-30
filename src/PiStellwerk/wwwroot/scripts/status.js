import * as User from "./user.js";
import * as Util from "./util.js";
let isRunning = false;
let isBlocked;
let statusIntervalHandle;
const commandButton = document.getElementById("CommandButton");
const commandTitle = document.getElementById("CommandTitle");
const commandDetails = document.getElementById("CommandDetails");
document.addEventListener("DOMContentLoaded", async () => {
    console.log("Status module initialization started");
    commandButton.addEventListener("click", commandButtonPressed);
    startStatusUpdates();
    console.log("Status module initialization was completed");
    while (true) {
        const result = await fetch(`/status?lastknownstatus=${isRunning.toString()}`, Util.getRequestInit("GET"));
        if (result.ok) {
            const json = await result.json();
            isRunning = json.isRunning;
            handleStatusChange(json.lastActionUsername);
        }
    }
});
async function commandButtonPressed() {
    if (isBlocked) {
        return;
    }
    postStatusChange(!isRunning);
    isRunning = (!isRunning);
    handleStatusChange(`${User.getUsername()} (You)`);
}
export function startStatusUpdates() {
    statusIntervalHandle = window.setInterval(() => regularUpdate(), 5000);
}
export function stopStatusUpdates() {
    clearInterval(statusIntervalHandle);
}
function handleStatusChange(username) {
    if (isRunning) {
        commandButton.classList.remove("has-background-danger");
        commandButton.classList.add("has-background-primary");
        commandTitle.innerHTML = "RUNNING";
        commandDetails.innerHTML = `PiStellwerk started by ${username} `;
    }
    else {
        isBlocked = true;
        commandButton.classList.remove("has-background-primary");
        commandButton.classList.add("has-background-danger-light");
        commandTitle.innerHTML = "STOPPED (LOCKED)";
        commandDetails.innerHTML = `PiStellwerk stopped by ${username} `;
        setTimeout(() => {
            isBlocked = false;
            commandButton.classList.remove("has-background-danger-light");
            commandTitle.innerHTML = "STOPPED";
            commandButton.classList.add("has-background-danger");
        }, 2500);
    }
}
async function postStatusChange(isRunning) {
    let bodyObject = { isRunning: isRunning, lastActionUsername: User.getUsername() };
    let bodyContent = JSON.stringify(bodyObject);
    fetch("/status", Util.getRequestInit("POST", bodyContent));
}
function regularUpdate() {
    let bodyContent = JSON.stringify({ name: User.getUsername(), UserAgent: navigator.userAgent });
    fetch("/status", Util.getRequestInit("PUT", bodyContent));
}
//# sourceMappingURL=status.js.map