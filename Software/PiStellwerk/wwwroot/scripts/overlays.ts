export function toggleVisibility(overlay: HTMLElement): void {
    if (overlay.style.display === "flex") {
        overlay.style.display = "none";
    } else {
        overlay.style.display = "flex";
    };
}