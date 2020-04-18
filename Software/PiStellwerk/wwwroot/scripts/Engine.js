"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var DisplayTypeAttribute = "data-display-type";
function TESTLoadEngines() {
    let Container = document.getElementById("EngineContainer");
    fetch("/engine/list", {
        method: "GET",
        headers: {
            'Content-Type': 'application/json'
        },
    }).then((response) => {
        return response.json();
    }).then((data) => {
        data.forEach(engine => {
            var tempnode = document.getElementById("EngineTemplate").cloneNode(true);
            tempnode.querySelector("header").textContent = engine.name;
            tempnode.style.display = "block";
            let tempinput = tempnode.querySelector("input");
            tempinput.addEventListener("input", HandleRangeValueChanged);
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
            tempinput.setAttribute(DisplayTypeAttribute, engine.speedDisplayType);
            Container.appendChild(tempnode);
        });
    });
}
exports.TESTLoadEngines = TESTLoadEngines;
function HandleRangeValueChanged(event) {
    let targetElement = event.target;
    writeSpeed(targetElement.parentElement.querySelector("output"), targetElement.value, targetElement.getAttribute(DisplayTypeAttribute));
}
function writeSpeed(output, value, displayType) {
    switch (displayType) {
        case "Percent":
            output.textContent = `${value}%`;
            break;
        case "SpeedSteps":
            output.textContent = value;
            break;
        case "TopSpeed":
            output.textContent = `${value} km/h`;
    }
}
//# sourceMappingURL=Engine.js.map