document.addEventListener('DOMContentLoaded', () => {
    // navbar active element highlighting
    document.querySelectorAll('.menu-link').forEach(link => { 
        var elementText = link.innerText.trim().replace(/\s/g, "").toLowerCase();
        var currentLocation = location.pathname.split('/')[1].toLowerCase();
        if (elementText === currentLocation) {
            link.classList.add('active');
            link.classList.remove('text-white');
            link.style.backgroundColor = "#eeeeee";
        }
    });
})


