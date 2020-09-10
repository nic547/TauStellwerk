export function toggleVisibility(overlay: HTMLElement): void {
    if (overlay.style.display === "block") {
        overlay.style.display = "none";
    } else {
        overlay.style.display = "block";
    };
}