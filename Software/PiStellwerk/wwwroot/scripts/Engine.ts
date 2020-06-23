var DisplayTypeAttribute: string = "data-display-type";
var EngineIDAttribute: string = "data-engine-id";

document.addEventListener("DOMContentLoaded",
    () => {
        document.getElementById("TestButton").addEventListener("click", TESTLoadEngines);
    });

export function TESTLoadEngines() {
        let Container: HTMLDivElement = <HTMLDivElement>document.getElementById("EngineContainer")

        fetch("/engine/list",
            {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json'
                },
            }

        ).then((response) => {
            return response.json();
        }).then((data: Array<any>) => {
            data.forEach(engine => {
                var tempnode: HTMLDivElement = <HTMLDivElement>document.getElementById("EngineTemplate").cloneNode(true);
                tempnode.querySelector("header").textContent = engine.name;
                tempnode.setAttribute(EngineIDAttribute, engine.id)
                let tempinput = tempnode.querySelector("input") as HTMLInputElement;
                tempinput.addEventListener("input", HandleRangeValueChanged)
                switch (engine.speedDisplayType) {
                    case "Percent":
                        tempnode.querySelector("output").textContent = `0%`;
                        tempinput.max = "100";
                        break;
                    case "SpeedSteps":
                        tempnode.querySelector("output").textContent = "0";
                        tempinput.max = engine.speedSteps;
                        break;
                    case "TopSpeed":
                        tempnode.querySelector("output").textContent = `0 km/h`;
                        tempinput.max = engine.topSpeed;
                }
                tempinput.setAttribute(DisplayTypeAttribute, engine.speedDisplayType)
                Container.appendChild(tempnode);
            })
            
        });
}

function HandleRangeValueChanged(event) {
    let targetElement = event.target as HTMLInputElement;
    writeSpeed(targetElement.parentElement.querySelector("output"), targetElement.value, targetElement.getAttribute(DisplayTypeAttribute));
    
    fetch(`/engine/command/${targetElement.parentElement.getAttribute(EngineIDAttribute)}`,
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
        }

    ).then((response) => { });
}

function writeSpeed(output: HTMLOutputElement, value, displayType: string) {
    switch (displayType) {
        case "Percent":
            output.textContent = `${value}%`;
            break;
        case "SpeedSteps":
            output.textContent = value;
            break;
        case "TopSpeed":
            output.textContent = `${value} km/h`
    }
}