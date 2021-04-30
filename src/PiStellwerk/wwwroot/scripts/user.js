var username;
var sessionId = "";
const overlay = document.getElementById("UsernameOverlay");
const usernameLabel = document.getElementById("UsernameLabel");
const usernameForm = document.getElementById("UsernameForm");
const usernameInput = document.getElementById("UsernameInput");
import * as Overlays from "./overlays.js";
import * as Util from "./util.js";
document.addEventListener("DOMContentLoaded", async () => {
    console.log("User module initializing");
    usernameLabel.addEventListener("click", () => { Overlays.toggleVisibility(overlay); });
    usernameForm.addEventListener("submit", () => handleSubmit(event));
    setUsername(Math.floor(Math.random() * 10000000).toString());
    var result = await fetch("/session", Util.getRequestInit("POST", `"${username}"`));
    sessionId = await result.text();
    console.log("User module finished initializing");
});
export function getUsername() {
    return username;
}
export function getSessionId() {
    return sessionId;
}
function handleSubmit(event) {
    event.preventDefault();
    let newUsername = usernameInput.value;
    usernameForm.reset();
    setUsername(newUsername);
    Overlays.toggleVisibility(overlay);
}
function setUsername(newUsername) {
    if (username != undefined) {
        fetch("/session", Util.getRequestInit("PUT", JSON.stringify(newUsername)));
    }
    username = newUsername;
    usernameLabel.textContent = `User: ${username}`;
}
//# sourceMappingURL=user.js.map