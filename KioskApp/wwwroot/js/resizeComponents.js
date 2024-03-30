function resizeComponents() {
    const designWidth = parseInt(document.querySelector('#canvas-container').dataset.width.replace(/[^\d.]/g, ''), 10);
    const designHeight = parseInt(document.querySelector('#canvas-container').dataset.height.replace(/[^\d.]/g, ''), 10);
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    const scaleFactorWidth = viewportWidth / designWidth;
    const scaleFactorHeight = viewportHeight / designHeight;
    const components = document.querySelectorAll('.resize-drag');

    components.forEach((comp) => {
        const originalWidth = parseFloat(comp.dataset.width);
        const originalHeight = parseFloat(comp.dataset.height);
        const originalPosX = parseFloat(comp.dataset.posx);
        const originalPosY = parseFloat(comp.dataset.posy);
        comp.style.width = `${originalWidth * scaleFactorWidth}px`;
        comp.style.height = `${originalHeight * scaleFactorHeight}px`;
        comp.style.left = `${originalPosX * scaleFactorWidth}px`;
        comp.style.top = `${originalPosY * scaleFactorHeight}px`;
    });
}

window.addEventListener('resize', resizeComponents);
