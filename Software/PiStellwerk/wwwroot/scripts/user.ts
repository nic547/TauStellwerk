var username: string;
var sessionId = "";

const overlay = document.getElementById("UsernameOverlay");
const usernameLabel = document.getElementById("UsernameLabel") as HTMLLabelElement;
const usernameForm = document.getElementById("UsernameForm") as HTMLFormElement;
const usernameInput = document.getElementById("UsernameInput") as HTMLInputElement;

import * as Overlays from "./overlays.js"
import * as Util from "./util.js"

document.addEventListener("DOMContentLoaded",
    () => {
        console.log("User module initializing");
        usernameLabel.addEventListener("click", () => { Overlays.toggleVisibility(overlay) });
        usernameForm.addEventListener("submit", () => handleSubmit(event));

        setUsername(Math.floor(Math.random() * 10000000).toString());

        fetch("/session", Util.getRequestInit("POST", `"${username}"`));

        console.log("User module finished initializing");
    });

export function getUsername(): string {
    return username;
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
        fetch("/user", Util.getRequestInit("PUT", JSON.stringify(newUsername)));
    }
    username = newUsername;
    usernameLabel.innerHTML = `User: ${username}`;

    
}