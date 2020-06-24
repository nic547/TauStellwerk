export function toggleVisibility(overlayId) {
    let overlay = document.getElementById(overlayId);
    if (overlay.style.display === "block") {
        overlay.style.display = "none";
    }
    else {
        overlay.style.display = "block";
    }
    ;
}
//# sourceMappingURL=overlays.js.map