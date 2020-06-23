export function toggleVisibility(overlayId) {
    let overlay = document.getElementById(overlayId);
    if (overlay.style.display === "none") {
        overlay.style.display = "block";
    }
    else {
        overlay.style.display = "none";
    }
    ;
}
//# sourceMappingURL=overlays.js.map