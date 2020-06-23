export function toggleVisibility(overlayId: string): void {
    let overlay = document.getElementById(overlayId);
    if (overlay.style.display === "none") {
        overlay.style.display = "block";
    } else {
        overlay.style.display = "none";

    };
}