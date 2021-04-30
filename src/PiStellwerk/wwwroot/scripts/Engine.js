import * as Overlays from "./overlays.js";
import * as Util from "./util.js";
const engineIdAttribute = "data-engine-id";
const functionNumberAttribute = "data-function-number";
//Elements
const controlTemplate = document.getElementById("EngineTemplate");
const selectionTemplate = document.getElementById("EngineSelectionTemplate");
const selectionButton = document.getElementById("SelectEngineButton");
const selectionOverlay = document.getElementById("EngineSelectionModal");
const selectionCloseButton = document.getElementById("EngineSelectionClose");
const selectionContainer = document.getElementById("EngineSelectionContainer");
var currentPage = 0;
document.addEventListener("DOMContentLoaded", () => {
    selectionButton.addEventListener("click", openEngineSelectionModal);
    selectionCloseButton.addEventListener("click", closeEngineSelectionModal);
});
async function choseEngineFromSelection() {
    const element = this;
    const id = element.getAttribute(engineIdAttribute);
    const acquireResult = await fetch(`/engine/${id}/acquire`, Util.getRequestInit("POST"));
    if (acquireResult.status === 423) {
        alert("Cannot acquire engine, already in use!");
        return;
    }
    if (!acquireResult.ok) {
        alert("Error while trying to acquire engine.");
        return;
    }
    const completeEngineResponse = await fetch(`/engine/${id}`, Util.getRequestInit("GET"));
    const engine = await completeEngineResponse.json();
    addEngineToControlPanel(engine);
    closeEngineSelectionModal();
}
function addEngineToControlPanel(engine) {
    const tempNode = controlTemplate.cloneNode(true);
    const container = document.getElementById("EngineContainer");
    tempNode.querySelector("span").innerHTML = engine.name;
    tempNode.setAttribute(engineIdAttribute, engine.id.toString());
    var picture = tempNode.querySelector("picture");
    addEngineImagesToPicture(picture, engine);
    tempNode.classList.remove("template");
    tempNode.removeAttribute("id");
    const tempInput = tempNode.querySelector("input");
    tempInput.addEventListener("input", handleEngineThrottleChange);
    tempNode.getElementsByTagName("button")[0].addEventListener("click", removeEngineFromControlPanel);
    const functionContainer = tempNode.getElementsByClassName("ec-grid-functions")[0];
    engine.functions.sort((a, b) => a.number - b.number).forEach(func => {
        const button = document.createElement("button");
        button.classList.add("button", "is-fullwidth", "has-left-text");
        button.setAttribute(functionNumberAttribute, func.number.toString());
        button.innerHTML = `F${func.number} - ${func.name !== "" ? func.name : "<span class=\"is-italic\">unnamed</span>"}`;
        button.addEventListener("click", handleFunctionButton);
        functionContainer.insertAdjacentElement("beforeend", button);
    });
    const directionButtons = tempNode.querySelector(".ec-grid-direction").children;
    for (let button of directionButtons) {
        button.addEventListener("click", handleEngineDirectionChange);
    }
    container.insertBefore(tempNode, container.lastElementChild);
}
async function handleEngineThrottleChange(event) {
    const targetElement = event.target;
    const speedStep = targetElement.value;
    const engineId = getEngineId(targetElement);
    targetElement.parentElement.parentElement.querySelector("output").innerHTML = speedStep;
    await fetch(`/engine/${engineId}/speed/${speedStep}`, Util.getRequestInit("POST"));
}
async function handleEngineDirectionChange() {
    const parent = this.parentElement;
    const targetButton = this;
    const id = getEngineId(parent);
    let forward;
    let otherButton;
    if (parent.children[0] === this) {
        forward = false;
        otherButton = parent.children[1];
    }
    else {
        forward = true;
        otherButton = parent.children[0];
    }
    targetButton.disabled = true;
    otherButton.disabled = false;
    await fetch(`/engine/${id}/speed/0?forward=${forward}`, Util.getRequestInit("POST"));
}
async function handleFunctionButton() {
    const targetElement = this;
    const number = targetElement.getAttribute(functionNumberAttribute);
    const id = targetElement.parentElement.parentElement.getAttribute(engineIdAttribute);
    let state;
    if (targetElement.classList.contains("has-background-primary-light")) {
        state = "off";
        targetElement.classList.remove("has-background-primary-light");
    }
    else {
        state = "on";
        targetElement.classList.add("has-background-primary-light");
    }
    await fetch(`/engine/${id}/function/${number}/${state}`, Util.getRequestInit("POST"));
}
async function removeEngineFromControlPanel() {
    const element = this;
    const engineId = getEngineId(element);
    await fetch(`/engine/${engineId}/release`, Util.getRequestInit("POST"));
    element.parentElement.parentElement.remove();
}
function openEngineSelectionModal() {
    Overlays.toggleVisibility(selectionOverlay);
    currentPage = 0;
    refreshContent();
}
;
function addEngineToEngineSelection(engine) {
    const tempNode = selectionTemplate.cloneNode(true);
    const titleElement = tempNode.children[0];
    const imageElement = tempNode.querySelector("picture");
    const tagsElement = tempNode.children[2];
    titleElement.innerHTML = engine.name;
    addEngineImagesToPicture(imageElement, engine);
    tagsElement.innerHTML = engine.tags.map(tag => `<span>${tag}</span>`).join("");
    tempNode.addEventListener("click", choseEngineFromSelection);
    tempNode.setAttribute(engineIdAttribute, engine.id.toString());
    tempNode.classList.remove("template");
    tempNode.removeAttribute("id");
    selectionContainer.appendChild(tempNode);
}
function clearEngineSelection() {
    const length = selectionContainer.children.length;
    for (let i = length - 1; i > 0; i--) { // Element 0 is ignored deliberately
        selectionContainer.children[i].remove();
    }
}
async function loadEnginesFromServer(page) {
    const response = await fetch(`/engine/list?page=${page}`, Util.getRequestInit("GET"));
    return await response.json();
}
function closeEngineSelectionModal() {
    Overlays.toggleVisibility(selectionOverlay);
    clearEngineSelection();
}
async function refreshContent() {
    const items = await loadEnginesFromServer(currentPage);
    clearEngineSelection();
    items.forEach(engine => {
        addEngineToEngineSelection(engine);
    });
}
class Engine {
}
class DccFunction {
}
class EngineImage {
}
class GroupedImage {
    constructor(type) {
        this.type = type;
        this.images = new Array();
    }
}
/**
 * Try and get the engineIdAttribute from an element or it's parent elements.
 * Should be more "solid" than just chaining the correct amount of parentElement.
 * @param element
 */
function getEngineId(element) {
    while (!element.hasAttribute(engineIdAttribute)) {
        if (element.parentElement !== undefined) {
            element = element.parentElement;
        }
        else {
            return null;
        }
    }
    return parseInt(element.getAttribute(engineIdAttribute));
}
function addEngineImagesToPicture(element, engine) {
    if (engine.image.length === 0) {
        return;
    }
    var groups = groupImages(engine.image);
    for (const group of groups) {
        const sizes = 'sizes="(min-width: 769px) 50vw,(min-width: 1216px) 25vw,(min-width: 1408px) 20vw,100vw"';
        const srcsetElement = group.images.map(i => `engineimages/${i.filename} ${i.width}w`);
        const srcset = srcsetElement.join(",");
        element.insertAdjacentHTML("afterbegin", `<source ${sizes} srcset="${srcset}" type="${group.type}">`);
    }
}
function groupImages(images) {
    const result = new Array();
    for (let image of images) {
        let group = result.find(g => g.type === image.type);
        if (group == null) {
            group = new GroupedImage(image.type);
            result.push(group);
        }
        group.images.push(image);
    }
    return result;
}
//# sourceMappingURL=Engine.js.map