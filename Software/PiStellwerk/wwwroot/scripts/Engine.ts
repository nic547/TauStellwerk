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

document.addEventListener("DOMContentLoaded",
    () => {
        document.getElementById("TestButton").addEventListener("click", TESTLoadEngines);
        selectionButton.addEventListener("click", openSelection);

        selectionCloseButton.addEventListener("click", closeSelection);
    });



export function TESTLoadEngines() {
        

        fetch("/engine/list",
            {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                },
            }

        ).then((response) => {
            return response.json();
        }).then((data: Array<any>) => {
            data.forEach(engine => displayEngine(engine));

        });
}

function selectEngine() {
    const element = this as HTMLElement;
    const id = element.getAttribute(engineIdAttribute);
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
    
    fetch(`/engine/command/${targetElement.parentElement.getAttribute(engineIdAttribute)}`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
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

function removeEngineFromControlPanel () {
    const element = event.target as HTMLElement;
    element.parentElement.parentElement.remove();
}

function openSelection() {
    Overlays.toggleVisibility(selectionOverlay);
    fetch("/engine/list",
        {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        }

    ).then((response) => {
        return response.json();
    }).then((data: Array<any>) => {
        data.forEach(engine => {
            const tempNode = selectionTemplate.cloneNode(true) as HTMLElement;
            var titleElement = tempNode.children[0] as HTMLElement;
            var tagsElement = tempNode.children[1] as HTMLElement;

            titleElement.innerHTML = engine.name;
            tagsElement.innerHTML = engine.tags.reduce((a, b) => { return a + " | " + b });

            tempNode.addEventListener("click", selectEngine);
            tempNode.setAttribute(engineIdAttribute, engine.id);
            
            tempNode.classList.remove("template");
            tempNode.removeAttribute("id");
            selectionContainer.appendChild(tempNode);
        });

    });
}

function closeSelection() {
    Overlays.toggleVisibility(selectionOverlay);
    const length = selectionContainer.children.length;
    for (let i = length -1; i > 0; i--) { // Element 0 is ignored deliberately
        selectionContainer.children[i].remove();
    }
}

export function getEngineSelectionCapacity(): number {
    const style = getComputedStyle(selectionContainer);
    const columns = style.gridTemplateColumns.split(" ").length;
    const rows = style.gridTemplateRows.split(" ").length;
    return rows * columns;
}