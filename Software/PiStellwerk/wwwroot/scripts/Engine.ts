const displayTypeAttribute = "data-display-type";
const engineIdAttribute = "data-engine-id";
const engineControlTemplate = document.getElementById("EngineTemplate") as HTMLDivElement;

// Element IDs

const engineSelectionId = "EngineSelectionOverlay";


document.addEventListener("DOMContentLoaded",
    () => {
        document.getElementById("TestButton").addEventListener("click", TESTLoadEngines);
    });


import * as Overlays from "./overlays.js"

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
    let tempInput = tempNode.querySelector("input") as HTMLInputElement;
    tempInput.addEventListener("input", HandleRangeValueChanged);
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

function HandleRangeValueChanged(event) {
    let targetElement = event.target as HTMLInputElement;
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