var username;
const overlayId = "UsernameOverlay";
const usernameLabelId = "UsernameLabel";
const usernameFormId = "UsernameForm";
const usernameInputId = "UsernameInput";
import * as Overlays from "./overlays.js";
document.addEventListener("DOMContentLoaded", () => {
    console.log("User module initializing");
    document.getElementById(usernameLabelId).addEventListener("click", () => { Overlays.toggleVisibility(overlayId); });
    document.getElementById(usernameFormId).addEventListener("submit", () => handleSubmit(event));
    setUsername(Math.floor(Math.random() * 10000000).toString());
    console.log("User module finished initializing");
});
export function getUsername() {
    return username;
}
function handleSubmit(event) {
    event.preventDefault();
    let inputElement = document.getElementById(usernameInputId);
    let inputForm = document.getElementById(usernameFormId);
    let newUsername = inputElement.value;
    inputForm.reset();
    setUsername(newUsername);
    Overlays.toggleVisibility(overlayId);
}
function setUsername(newUsername) {
    let oldUsername = username;
    username = newUsername;
    document.getElementById(usernameLabelId).innerHTML = `User: ${username}`;
    if (oldUsername != undefined) {
        fetch("/user", {
            method: "PUT",
            headers: {
                'Content-Type': "application/json"
            },
            body: JSON.stringify([
                { name: oldUsername, UserAgent: navigator.userAgent },
                { name: newUsername, UserAgent: navigator.userAgent }
            ])
        });
    }
}
//# sourceMappingURL=user.js.map