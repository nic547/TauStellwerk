import * as Overlays from "./overlays.js"

const displayTypeAttribute = "data-display-type";
const engineIdAttribute = "data-engine-id";

//Elements

const engineControlTemplate = document.getElementById("EngineTemplate") as HTMLDivElement;
const engineSelectionTemplate = document.getElementById("EngineSelectionTemplate") as HTMLElement;

const engineSelectionButton = document.getElementById("SelectEngineButton") as HTMLDivElement;
const engineSelectionOverlay = document.getElementById("EngineSelectionOverlay") as HTMLDivElement;

const engineSelectionContainer = document.getElementById("EngineSelectionContainer") as HTMLDivElement;

document.addEventListener("DOMContentLoaded",
    () => {
        document.getElementById("TestButton").addEventListener("click", TESTLoadEngines);
        engineSelectionButton.addEventListener("click", openEngineSelection);
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

function displayEngine(engine: any) {
    const tempNode = engineControlTemplate.cloneNode(true) as HTMLDivElement;
    const container = document.getElementById("EngineContainer") as HTMLDivElement;

    tempNode.querySelector("header").innerHTML = engine.name;
    tempNode.setAttribute(engineIdAttribute, engine.id);
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

    tempInput.setAttribute(displayTypeAttribute, engine.speedDisplayType)
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

function openEngineSelection() {
    Overlays.toggleVisibility(engineSelectionOverlay);
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
            const tempNode = engineSelectionTemplate.cloneNode(true) as HTMLElement;
            var titleElement = tempNode.children[0] as HTMLElement;
            var tagsElement = tempNode.children[1] as HTMLElement;

            titleElement.innerHTML = engine.name;
            tagsElement.innerHTML = engine.tags.reduce((a, b) => { return a + " | " + b });

            
            tempNode.classList.remove("template");
            engineSelectionContainer.appendChild(tempNode);
        });

    });
}

export function getEngineSelectionCapacity(): number {
    const style = getComputedStyle(engineSelectionContainer);
    const columns = style.gridTemplateColumns.split(" ").length;
    const rows = style.gridTemplateRows.split(" ").length;
    return rows * columns;
}