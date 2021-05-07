import * as Overlays from "./overlays.js"
import * as Util from "./util.js"

const engineIdAttribute = "data-engine-id";
const functionNumberAttribute = "data-function-number";

let currentPage = 0;

//Elements

const controlTemplate = document.getElementById("EngineTemplate") as HTMLDivElement;
const selectionTemplate = document.getElementById("EngineSelectionTemplate") as HTMLElement;

const selectionButton = document.getElementById("SelectEngineButton") as HTMLDivElement;

const selectionOverlay = document.getElementById("EngineSelectionModal") as HTMLDivElement;
const selectionCloseButton = document.getElementById("EngineSelectionClose") as HTMLDivElement;
const selectionContainer = document.getElementById("EngineSelectionContainer") as HTMLDivElement;

const forwardsButton = document.getElementById("EngineSelectionForward") as HTMLButtonElement;
const backwardsButton = document.getElementById("EngineSelectionBackwards") as HTMLButtonElement;


document.addEventListener("DOMContentLoaded",
    () => {
        selectionButton.addEventListener("click", openEngineSelectionModal);

        selectionCloseButton.addEventListener("click", closeEngineSelectionModal);
        
        forwardsButton.addEventListener("click", () => {scrollPages(1)});
        backwardsButton.addEventListener("click", () => {scrollPages(-1)});
    });

async function choseEngineFromSelection() {
    const element = this as HTMLElement;
    const id = element.getAttribute(engineIdAttribute);

    const acquireResult = await fetch(`/engine/${id}/acquire`,Util.getRequestInit("POST"));

    if (acquireResult.status === 423) {
        alert("Cannot acquire engine, already in use!");
        return;
    }

    if (!acquireResult.ok) {
        alert("Error while trying to acquire engine.");
        return;
    }
    const completeEngineResponse = await fetch(`/engine/${id}`, Util.getRequestInit("GET"));
    const engine = await completeEngineResponse.json() as Engine;
    addEngineToControlPanel(engine);
    closeEngineSelectionModal();
}

function addEngineToControlPanel(engine: Engine) {
    const tempNode = controlTemplate.cloneNode(true) as HTMLDivElement;
    const container = document.getElementById("EngineContainer") as HTMLDivElement;

    tempNode.querySelector("span").innerHTML = engine.name;
    tempNode.setAttribute(engineIdAttribute, engine.id.toString());

    var picture = (tempNode.querySelector("picture") as HTMLPictureElement);
    addEngineImagesToPicture(picture, engine);

    tempNode.classList.remove("template");
    tempNode.removeAttribute("id");

    const tempInput = tempNode.querySelector("input") as HTMLInputElement;
    tempInput.addEventListener("input", handleEngineThrottleChange);

    tempNode.getElementsByTagName("button")[0].addEventListener("click", removeEngineFromControlPanel);

    const functionContainer = tempNode.getElementsByClassName("ec-grid-functions")[0] as HTMLDivElement;

    engine.functions.sort((a, b) => a.number - b.number).forEach(func => {
        const button = document.createElement("button");
        button.classList.add("button", "is-fullwidth", "has-left-text");
        button.setAttribute(functionNumberAttribute, func.number.toString());
        button.innerHTML = `F${func.number} - ${func.name !== "" ? func.name : "<span class=\"is-italic\">unnamed</span>"}`;

        button.addEventListener("click",handleFunctionButton);

        functionContainer.insertAdjacentElement("beforeend", button);
    });

    const directionButtons = tempNode.querySelector(".ec-grid-direction").children;
    for (let button of directionButtons) {
        button.addEventListener("click", handleEngineDirectionChange);
    }
   

    container.insertBefore(tempNode,container.lastElementChild);
}
async function handleEngineThrottleChange(event): Promise<void> {
    const targetElement = event.target as HTMLInputElement;

    const speedStep = targetElement.value;
    const engineId = getEngineId(targetElement);

    targetElement.parentElement.parentElement.querySelector("output").innerHTML = speedStep;

    await fetch(`/engine/${engineId}/speed/${speedStep}`, Util.getRequestInit("POST"));
}

async function handleEngineDirectionChange(): Promise<void> {
    const parent = this.parentElement as HTMLDivElement;
    const targetButton = this as HTMLButtonElement;
    const id = getEngineId(parent);
    let forward: boolean;
    let otherButton: HTMLButtonElement;


    if (parent.children[0] === this) {
        forward = false;
        otherButton = parent.children[1] as HTMLButtonElement;
    } else {
        forward = true;
        otherButton = parent.children[0] as HTMLButtonElement;
    }

    targetButton.disabled = true;
    otherButton.disabled = false;
    await fetch(`/engine/${id}/speed/0?forward=${forward}`, Util.getRequestInit("POST"));
}

async function handleFunctionButton(): Promise<void> {
    const targetElement = this;
    const number = targetElement.getAttribute(functionNumberAttribute);
    const id = targetElement.parentElement.parentElement.getAttribute(engineIdAttribute);
    let state: string;

    if (targetElement.classList.contains("has-background-primary-light")) {
        state = "off";
        targetElement.classList.remove("has-background-primary-light");
    } else {
        state = "on";
        targetElement.classList.add("has-background-primary-light");
    }

    await fetch(`/engine/${id}/function/${number}/${state}`, Util.getRequestInit("POST"));
}

async function removeEngineFromControlPanel () {
    const element = this as HTMLElement;

    const engineId = getEngineId(element);

    await fetch(`/engine/${engineId}/release`,Util.getRequestInit("POST"));

    element.parentElement.parentElement.remove();
}

function openEngineSelectionModal() {
    Overlays.toggleVisibility(selectionOverlay);

    currentPage = 0;
    refreshContent();

}

function addEngineToEngineSelection(engine: Engine) {
    const tempNode = selectionTemplate.cloneNode(true) as HTMLElement;
    const titleElement = tempNode.children[0] as HTMLElement;
    const imageElement = tempNode.querySelector("picture") as HTMLPictureElement;
    const tagsElement = tempNode.children[2] as HTMLElement;

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

async function loadEnginesFromServer(page: number): Promise<Engine[]> {


    const response = await fetch(`/engine/list?page=${page}`, Util.getRequestInit("GET"));
    return await response.json();
}


function closeEngineSelectionModal() {
    Overlays.toggleVisibility(selectionOverlay);
    currentPage = 0;
    clearEngineSelection();
}

async function scrollPages(pages: number) {
    currentPage+= pages;
    
    if (currentPage < 0) {
        currentPage = 0;
    }
    
    backwardsButton.disabled = currentPage === 0;
    
    const count = await refreshContent();
    forwardsButton.disabled = count === 0;
}


async function refreshContent(): Promise<number> {
    const items = await loadEnginesFromServer(currentPage);

    clearEngineSelection();
    items.forEach(engine => {
        addEngineToEngineSelection(engine);
    });
    
    return items.length;
}

class Engine {
    name: string;
    id: number;
    image: EngineImage[];
    tags: string[];
    functions: DccFunction[];
}

class DccFunction {
    name: string;
    number: number;
    id: number;
}

class EngineImage {
    filename: string;
    type: string;
    importance: number;
    width: number;
}

class GroupedImage {
    type: string;
    images: EngineImage[];

    constructor(type: string) {
        this.type = type;
        this.images = new Array<EngineImage>();
    }
}


/**
 * Try and get the engineIdAttribute from an element or it's parent elements.
 * Should be more "solid" than just chaining the correct amount of parentElement.
 * @param element
 */
function getEngineId(element: HTMLElement): number {
    while (!element.hasAttribute(engineIdAttribute)) {
        if (element.parentElement !== undefined) {
            element = element.parentElement;
        } else {
            return null;
        }
    }
    return parseInt(element.getAttribute(engineIdAttribute));
}

function addEngineImagesToPicture(element: HTMLPictureElement, engine: Engine): void {
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

function groupImages(images: EngineImage[]): GroupedImage[] {
    const result = new Array<GroupedImage>();

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