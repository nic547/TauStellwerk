export function toggleVisibility(modalElement: HTMLElement): void {
    if (modalElement.classList.contains("is-active")) {
        modalElement.classList.remove("is-active");
    } else {
        modalElement.classList.add("is-active");
    };
}