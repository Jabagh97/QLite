

localStorage.setItem('ServiceTimeoutActive', 'false');


function scaleTicketComponents() {
    // Get the ticket body dimensions
    const ticketBody = document.getElementById('ticket-body');
    const ticketWidth = ticketBody.offsetWidth;
    const ticketHeight = ticketBody.offsetHeight;

    // Design dimensions from the model
    const designWidth = parseInt(document.querySelector('#ticket-body').dataset.width.replace(/[^\d.]/g, ''), 10);
    const designHeight = parseInt(document.querySelector('#ticket-body').dataset.height.replace(/[^\d.]/g, ''), 10);

    // Scaling factors for width and height based on the ticket-body size
    const scaleFactorWidth = ticketWidth / designWidth;
    const scaleFactorHeight = ticketHeight / designHeight;

    // Select all resize-drag elements within the ticket-body
    const components = ticketBody.querySelectorAll('.resize-drag');
    components.forEach((comp) => {
        const originalWidth = parseFloat(comp.getAttribute('data-Width').replace('px', ''));
        const originalHeight = parseFloat(comp.getAttribute('data-Height').replace('px', ''));
        const originalPosX = parseFloat(comp.getAttribute('data-PosX').replace('px', ''));
        const originalPosY = parseFloat(comp.getAttribute('data-PosY').replace('px', ''));

        // Calculate new dimensions and positions based on scaling factors
        const newWidth = originalWidth * scaleFactorWidth;
        const newHeight = originalHeight * scaleFactorHeight;
        const newPosX = originalPosX * scaleFactorWidth;
        const newPosY = originalPosY * scaleFactorHeight;

        // Apply the new styles
        comp.style.width = `${newWidth}px`;
        comp.style.height = `${newHeight}px`;
        comp.style.left = `${newPosX}px`;
        comp.style.top = `${newPosY}px`;
        comp.style.position = 'absolute';
    });
}
scaleTicketComponents();
window.addEventListener('resize', scaleTicketComponents);

