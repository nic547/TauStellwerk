var username: string;

const overlay = document.getElementById("UsernameOverlay");
const usernameLabel = document.getElementById("UsernameLabel") as HTMLLabelElement;
const usernameForm = document.getElementById("UsernameForm") as HTMLFormElement;
const usernameInput = document.getElementById("UsernameInput") as HTMLInputElement;

import * as Overlays from "./overlays.js"

document.addEventListener("DOMContentLoaded",
    () => {
        console.log("User module initializing");
        usernameLabel.addEventListener("click", () => { Overlays.toggleVisibility(overlay) });
        usernameForm.addEventListener("submit", () => handleSubmit(event));

        setUsername(Math.floor(Math.random() * 10000000).toString());

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
    let oldUsername = username;

    username = newUsername;
    usernameLabel.innerHTML = `User: ${username}`;

    if (oldUsername != undefined) {
        fetch("/user",
            {
                method: "PUT",
                headers: {
                    'Content-Type': "application/json"
                },
                body: JSON.stringify([
                    { name: oldUsername, UserAgent: navigator.userAgent },
                    { name: newUsername, UserAgent: navigator.userAgent }
                ])
            }
        );
    }
}