import * as Overlays from "./overlays.js"

const displayTypeAttribute = "data-display-type";
const engineIdAttribute = "data-engine-id";

//Elements

const controlTemplate = document.getElementById("EngineTemplate") as HTMLDivElement;
const selectionTemplate = document.getElementById("EngineSelectionTemplate") as HTMLElement;

const selectionButton = document.getElementById("SelectEngineButton") as HTMLDivElement;

const selectionOverlay = document.getElementById("EngineSelectionModal") as HTMLDivElement;
const selectionCloseButton = document.getElementById("EngineSelectionClose") as HTMLDivElement;
const selectionContainer = document.getElementById("EngineSelectionContainer") as HTMLDivElement;

var currentPage = 0;

document.addEventListener("DOMContentLoaded",
    () => {
        selectionButton.addEventListener("click", openSelection);

        selectionCloseButton.addEventListener("click", closeSelection);
    });

async function selectEngine() {
    const element = this as HTMLElement;
    const id = element.getAttribute(engineIdAttribute);

    var acquireResult = await fetch(`/engine/${id}/acquire`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
        });

    if (acquireResult.status === 423) {
        alert("Cannot acquire engine, already in use!");
        return;
    }

    if (!acquireResult.ok) {
        alert("Error while trying to acquire engine.");
        return;
    }
    fetch(`/engine/${id}`,
        {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        }
    ).then((response) => {
        return response.json();
    }).then((engine => displayEngine(engine)));

closeSelection();
}

function displayEngine(engine: any) {
    const tempNode = controlTemplate.cloneNode(true) as HTMLDivElement;
    const container = document.getElementById("EngineContainer") as HTMLDivElement;

    tempNode.querySelector("span").innerHTML = engine.name;
    tempNode.setAttribute(engineIdAttribute, engine.id);

    tempNode.classList.remove("template");
    tempNode.removeAttribute("id");

    const tempInput = tempNode.querySelector("input") as HTMLInputElement;
    tempInput.addEventListener("input", handleRangeValueChanged);
    switch (engine.speedDisplayType) {
    case "Percent":
        tempNode.querySelector("output").innerHTML = `0%`;
        tempInput.max = "100";
        break;
    case "SpeedSteps":
        tempNode.querySelector("output").innerHTML = "0";
        tempInput.max = engine.speedSteps;
        break;
    case "TopSpeed":
        tempNode.querySelector("output").innerHTML = `0 km/h`;
        tempInput.max = engine.topSpeed;
    }

    tempNode.getElementsByClassName("button")[0].addEventListener("click", removeEngineFromControlPanel);

    let functionContainer = tempNode.getElementsByClassName("EngineFunctions")[0] as HTMLDivElement;

    engine.functions.sort((a, b) => a.number - b.number).forEach(func =>
        functionContainer.insertAdjacentHTML("beforeend", `<button class="button is-fullwidth has-text-left">F${func.number} - ${func.name !== "" ? func.name : "<span class=\"is-italic\">unnamed</span>"}</button>`));

    tempInput.setAttribute(displayTypeAttribute, engine.speedDisplayType);

   

    container.appendChild(tempNode);
}
function handleRangeValueChanged(event) {
    const targetElement = event.target as HTMLInputElement;
    writeSpeed(targetElement.parentElement.querySelector("output"), targetElement.value, targetElement.getAttribute(displayTypeAttribute));
    
    fetch(`/engine/${targetElement.parentElement.getAttribute(engineIdAttribute)}/command`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: `{"Type": "Speed","Data": ${targetElement.value}}`
        }

    ).then((response) => { });
}

function writeSpeed(output: HTMLOutputElement, value, displayType: string) {
    switch (displayType) {
        case "Percent":
            output.innerHTML = `${value}%`;
            break;
        case "SpeedSteps":
            output.innerHTML = value;
            break;
        case "TopSpeed":
            output.innerHTML = `${value} km/h`;
    }
}

async function removeEngineFromControlPanel () {
    const element = this as HTMLElement;

    const engineId = element.parentElement.parentElement.getAttribute(engineIdAttribute);

    await fetch(`/engine/${engineId}/release`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            }
        });

    element.parentElement.parentElement.remove();
}

function openSelection() {
    Overlays.toggleVisibility(selectionOverlay);

    currentPage = 0;
    refreshContent();

};

function addEngineToSelection(engine) {
    const tempNode = selectionTemplate.cloneNode(true) as HTMLElement;
    var titleElement = tempNode.children[0] as HTMLElement;
    var imageElement = tempNode.children[1].children[0] as HTMLImageElement;
    var tagsElement = tempNode.children[2] as HTMLElement;

    titleElement.innerHTML = engine.name;
    imageElement.setAttribute("src", engine.imageFileName ?? "/img/noImageImage.webP");
    tagsElement.innerHTML = engine.tags.map(tag => `<span class="tag is-rounded has-background-primary-light">${tag}</span>`).join("");

    tempNode.addEventListener("click", selectEngine);
    tempNode.setAttribute(engineIdAttribute, engine.id);

    tempNode.classList.remove("template");
    tempNode.removeAttribute("id");
    selectionContainer.appendChild(tempNode);
}

function clearSelection() {
    const length = selectionContainer.children.length;
    for (let i = length - 1; i > 0; i--) { // Element 0 is ignored deliberately
        selectionContainer.children[i].remove();
    }
}

async function loadList(page: Number): Promise<object[]> {


    var response = await fetch(`/engine/list?page=${page}`,
        {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        }
    );
    return await response.json();
}


function closeSelection() {
    Overlays.toggleVisibility(selectionOverlay);

    clearSelection();
}


async function refreshContent() {
    var items = await loadList(currentPage);

    clearSelection();
    items.forEach(engine => {
        addEngineToSelection(engine);
    });

}