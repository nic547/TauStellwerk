var username: string;
var sessionId = "";

const overlay = document.getElementById("UsernameOverlay");
const usernameLabel = document.getElementById("UsernameLabel") as HTMLButtonElement;
const usernameForm = document.getElementById("UsernameForm") as HTMLFormElement;
const usernameInput = document.getElementById("UsernameInput") as HTMLInputElement;

import * as Overlays from "./overlays.js"
import * as Util from "./util.js"

document.addEventListener("DOMContentLoaded",
    async () => {
        console.log("User module initializing");
        usernameLabel.addEventListener("click", () => { Overlays.toggleVisibility(overlay) });
        usernameForm.addEventListener("submit", () => handleSubmit(event));

        setUsername(Math.floor(Math.random() * 10000000).toString());

        var result = await fetch("/session", Util.getRequestInit("POST", `"${username}"`));
        sessionId = await result.text();

        console.log("User module finished initializing");
    });

export function getUsername(): string {
    return username;
}

export function getSessionId(): string {
    return sessionId;
}

function handleSubmit(event: Event) {
    event.preventDefault();
    let newUsername = usernameInput.value;
    usernameForm.reset();

    setUsername(newUsername);
    Overlays.toggleVisibility(overlay);
}


function setUsername(newUsername: string): void {
    if (username != undefined) {
        fetch("/session", Util.getRequestInit("PUT", JSON.stringify(newUsername)));
    }
    username = newUsername;
    usernameLabel.textContent = `User: ${username}`;

    
}