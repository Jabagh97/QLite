

$(document).ready(function () {
    // Check if an accordion item was previously open (has 'show' class)
    var isOpen = localStorage.getItem('accordionItemOpen');

    if (isOpen) {
        // Open the previously open accordion item
        $(isOpen).find('.menu-item here  menu-accordion').addClass('show');
    }

    // Handle accordion item click
    $('.menu-item here  menu-accordion').click(function () {
        // Remove 'show' class from all accordion items
        $('.menu-item here  menu-accordion').removeClass('show');

        // Add 'show' class to the clicked accordion item
        $(this).addClass('show');

        // Save the ID of the opened accordion item to localStorage
        localStorage.setItem('accordionItemOpen',  $(this).closest('.menu').attr('id'));
    });
});



$(document).ready(function () {
    // Check and set the sidebar_minimize_state
    var sidebarMinimizeState = localStorage.getItem('sidebar_minimize_state');

    if (sidebarMinimizeState === 'on') {
        // Set attributes and classes for minimized sidebar
        $('body').attr('data-kt-app-sidebar-minimize', 'on');
        $('#kt_app_sidebar_toggle').attr('data-kt-toggle-state', 'active').addClass('active');
    }
    else
    {
        // Remove attributes and classes for minimized sidebar
        $('body').attr('data-kt-app-sidebar-minimize', 'off');
        $(this).attr('data-kt-toggle-state', '').removeClass('active');
    }

    // Handle sidebar toggle click
    $('#kt_app_sidebar_toggle').click(function () {
        // Toggle the sidebar_minimize_state
        if (sidebarMinimizeState === 'on') {
            // If sidebar_minimize_state is on, set it to off
            sidebarMinimizeState = 'off';

            // Remove attributes and classes for minimized sidebar
            $('body').attr('data-kt-app-sidebar-minimize', 'off');
            $(this).attr('data-kt-toggle-state', '').removeClass('active');
        } else {
            // If sidebar_minimize_state is off, set it to on
            sidebarMinimizeState = 'on';

            // Set attributes and classes for minimized sidebar
            $('body').attr('data-kt-app-sidebar-minimize', 'on');
            $(this).attr('data-kt-toggle-state', 'active').addClass('active');
        }

        // Save the sidebar_minimize_state to localStorage
        localStorage.setItem('sidebar_minimize_state', sidebarMinimizeState);
    });
});





//Theme Method
$(document).ready(function () {
    var defaultThemeMode = "light";
    var themeMode;
    if (document.documentElement) {
        if (document.documentElement.hasAttribute("data-bs-theme-mode")) {
            themeMode = document.documentElement.getAttribute("data-bs-theme-mode");
        } else {
            if (localStorage.getItem("data-bs-theme") !== null) {
                themeMode = localStorage.getItem("data-bs-theme");
            } else {
                themeMode = defaultThemeMode;
            }
        }

        document.documentElement.setAttribute("data-bs-theme", themeMode);
    }
});

function decodeHtml(html) {
    var parser = new DOMParser();
    var doc = parser.parseFromString(html, 'text/html');
    return doc.documentElement.textContent;
}

function copyToClipboard2(text) {
    navigator.clipboard.writeText(text)
        .then(() => {
            showMetronicPopup('Content copied to clipboard!', 'success');
        })
        .catch(err => {
            showMetronicPopup('Unable to copy to clipboard', 'danger');
            console.error('Error during copy:', err);
        });
}

//Copy to CilpBoard Method 
function copyToClipboard(event, elementId) {
    // Prevent the default button click action
    event.preventDefault();

    // Get the text content of the element with the specified ID
    var element = document.getElementById(elementId);
    debugger;

    decodedText = decodeHtml(element.getInnerHTML())
    copyToClipboard2(decodedText);

}


function showMetronicPopup(message, type) {
    
    Swal.fire({
        icon: type, // success, error, warning, etc.
        title: message,
        timer: 5000, // Adjust the timer as needed
        showConfirmButton: false
    });
}

// Make the DIV element draggable:
var element = document.querySelector('#kt_modal_3');
dragElement(element);

function dragElement(elmnt) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (elmnt.querySelector('.modal-content')) {
        // if present, the header is where you move the DIV from:
        elmnt.querySelector('.modal-content').onmousedown = dragMouseDown;
    } else {
        // otherwise, move the DIV from anywhere inside the DIV:
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {
        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }

    function closeDragElement() {
        // stop moving when mouse button is released:
        document.onmouseup = null;
        document.onmousemove = null;
    }
}