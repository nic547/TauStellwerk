export function toggleVisibility(modalElement) {
    if (modalElement.classList.contains("active-modal")) {
        modalElement.classList.remove("active-modal");
    }
    else {
        modalElement.classList.add("active-modal");
    }
    ;
}
//# sourceMappingURL=overlays.js.map