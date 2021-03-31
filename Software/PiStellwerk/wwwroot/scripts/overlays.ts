export function toggleVisibility(modalElement: HTMLElement): void {
    if (modalElement.classList.contains("active-modal")) {
        modalElement.classList.remove("active-modal");
    } else {
        modalElement.classList.add("active-modal");
    };
}