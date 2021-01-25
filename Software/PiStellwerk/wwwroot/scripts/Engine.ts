import * as Overlays from "./overlays.js"

const displayTypeAttribute = "data-display-type";
const engineIdAttribute = "data-engine-id";

//Elements

const controlTemplate = document.getElementById("EngineTemplate") as HTMLDivElement;
const selectionTemplate = document.getElementById("EngineSelectionTemplate") as HTMLElement;

const selectionButton = document.getElementById("SelectEngineButton") as HTMLDivElement;

const selectionOverlay = document.getElementById("EngineSelectionOverlay") as HTMLDivElement;
const selectionCloseButton = document.getElementById("EngineSelectionClose") as HTMLDivElement;
const selectionContainer = document.getElementById("EngineSelectionContainer") as HTMLDivElement;

var startNumber;
var endNumber;
var currentCapacity: number;
const itemsPerPage = 20;

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

    tempNode.querySelector("header").innerHTML = engine.name;
    tempNode.setAttribute(engineIdAttribute, engine.id);

    tempNode.classList.remove("template");

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

    tempNode.getElementsByTagName("button")[0].addEventListener("click", removeEngineFromControlPanel);

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
    const element = event.target as HTMLElement;

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

    window.addEventListener("resize", handleSelectionResize);

    startNumber = 0;
    refreshContent();

};

function addEngineToSelection(engine) {
    const tempNode = selectionTemplate.cloneNode(true) as HTMLElement;
    var titleElement = tempNode.children[0] as HTMLElement;
    var tagsElement = tempNode.children[1] as HTMLElement;

    titleElement.innerHTML = engine.name;
    tagsElement.innerHTML = engine.tags.map(tag => `<span class="tag is-rounded is-dark">${tag}</span>`).join("");

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

    window.removeEventListener("resize", handleSelectionResize);

    clearSelection();
}

function handleSelectionResize() {
    const newCapacity = getEngineSelectionCapacity();

    if (newCapacity !== currentCapacity) {
        refreshContent();
        currentCapacity = newCapacity;
    }
}

async function refreshContent() {
    let capacity = getEngineSelectionCapacity();

    let pages = Math.ceil(capacity / itemsPerPage);
    if (startNumber % itemsPerPage !== 0) {
        pages += 1;
    }
    let startPage = Math.floor(startNumber / itemsPerPage);
    let promises = new Array<Promise<any[]>>();

    for (let i = startPage; i < (startPage + pages); i++) {
        promises.push(loadList(i));
    }

    var items = await Promise.all(promises);

    clearSelection();
    items.forEach(page =>
        page.forEach(engine => {
            if (startNumber <= engine.id && engine.id <= startNumber + capacity) {
                addEngineToSelection(engine);
            }
        })
    );

}

function getEngineSelectionCapacity(): number {
    const style = getComputedStyle(selectionContainer);
    return parseInt(style.getPropertyValue("--capacity"));
}