var username: string;
const overlayId: string = "UsernameOverlay";
const usernameLabelId: string = "UsernameLabel";
const usernameFormId: string = "UsernameForm";
const usernameInputId: string = "UsernameInput";

import * as Overlays from "./overlays.js"

export function getUsername(): string {
    return username;
}

export function init(): void {
    console.log("User module initializing");
    document.getElementById(usernameLabelId).addEventListener("click", () => { Overlays.toggleVisibility(overlayId) });

    document.getElementById(usernameFormId).addEventListener("submit", () => handleSubmit(event));

    setUsername(Math.floor(Math.random() * 10000000).toString());

    console.log("User module finished initializing");
}

function handleSubmit(event: Event) {
    event.preventDefault();
    let inputElement: HTMLInputElement = <HTMLInputElement>document.getElementById(usernameInputId);
    let newUsername: string = inputElement.value;
    inputElement.value = "";

    setUsername(newUsername);
    Overlays.toggleVisibility(overlayId);
}


function setUsername(newUsername: string): void {
    let oldUsername = username;

    username = newUsername;
    document.getElementById(usernameLabelId).innerHTML = `User: ${username}`;

    if (oldUsername != undefined) {
        fetch("/user",
            {
                method: "PUT",
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify([
                    { name: oldUsername, UserAgent: navigator.userAgent },
                    { name: newUsername, UserAgent: navigator.userAgent }
                ])
            }
        );
    }
}