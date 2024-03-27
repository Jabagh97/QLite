
//#region Init

dragAndDrop('resize-drag');

const loadingEl = document.createElement("div");
document.body.prepend(loadingEl);
loadingEl.classList.add("page-loader");
loadingEl.classList.add("flex-column");
loadingEl.innerHTML = `
        <span class="spinner-border text-primary" role="status"></span>
        <span class="text-muted fs-6 fw-semibold mt-5">Saving Changes Please wait...</span>
    `;

//#endregion


//#region Canvas

// Function to update canvas size in the desPageData object
function updateCanvasSize() {
    var widthSelect = document.getElementById('widthSelect');
    var heightSelect = document.getElementById('heightSelect');
    var selectedWidth = widthSelect.value;
    var selectedHeight = heightSelect.value;
    document.getElementById('canvas-container').style.width = selectedWidth;
    document.getElementById('canvas-container').style.height = selectedHeight;

    // Update desPageData properties
    desPageData.Width = selectedWidth;
    desPageData.Height = selectedHeight;
}

//// Function to update background color in the desPageData object
$("#backgroundColor").spectrum({
    color: "#ffffff",
    showInput: true,
    preferredFormat: "hex",
    showAlpha: true,
    change: function (color) {
        // Update background color
        $('#canvas-container').css('background-color', color.toHexString());

        // Update desPageData property
        desPageData.BackGroundColor = color.toHexString();
    }
});

// Function to handle file upload and update BgImageUrl property in the desPageData object
document.getElementById('imageUpload').addEventListener('change', function (event) {
    var file = event.target.files[0];
    var reader = new FileReader();
    reader.onload = function (event) {
        document.getElementById('canvas-container').style.backgroundImage = 'url(' + event.target.result + ')';
        // Update desPageData property
        desPageData.BgImageUrl = event.target.result;
    };
    reader.readAsDataURL(file);
});

// Event listeners for dropdown changes
document.getElementById('widthSelect').addEventListener('change', updateCanvasSize);
document.getElementById('heightSelect').addEventListener('change', updateCanvasSize);



// Retrieve the width and height values from the Model
var selectedWidth = desPageData.Width;
var selectedHeight = desPageData.Height;


// Select the width and height dropdowns
var widthSelect = document.getElementById('widthSelect');
var heightSelect = document.getElementById('heightSelect');

// Set the selected values for width and height dropdowns
widthSelect.value = selectedWidth;
heightSelect.value = selectedHeight;

function loadDesignImages() {

    desPageData.Comps.forEach(function (element) {
        if (element.DesCompType === "DesCompDataFrame") {
            getDesignByID(element.DesignId, element.Id);
        }
    });
}
loadDesignImages();


document.addEventListener('DOMContentLoaded', function () {
    // Set the input value to desPageData.PageTimeOut when the page loads
    var timeoutInput = document.getElementById('TimeOut');
    timeoutInput.value = desPageData.PageTimeOut;

    // Update desPageData.PageTimeOut whenever the input's value changes
    timeoutInput.addEventListener('input', function (e) {
        desPageData.PageTimeOut = parseInt(e.target.value, 10) || 0;
        console.log('Page Timeout set to:', desPageData.PageTimeOut); // For debugging purposes
    });
});


//#endregion


//#region Buttons

function createButton(buttonText, componentType, elementType) {
    // Create a new element based on the specified elementType
    var newComponent = document.createElement(elementType);
    var compId = 'comp_' + Date.now(); // Generate a unique ID for the component
    newComponent.id = compId; // Set the id attribute
    newComponent.style.width = '100px'; // Set default width
    newComponent.style.height = '100px'; // Set default height
    newComponent.className = 'resize-drag';
    newComponent.style.border = '1px dotted black';
    newComponent.style.transform = 'translate(30px, 30px)';

    newComponent.setAttribute('data-x', '30px');
    newComponent.setAttribute('data-y', '30px');

    newComponent.setAttribute('data-comp-id', compId);
    // Create a div element for the Text

    var textDiv = document.createElement('div');
    textDiv.id = compId + '_text';
    textDiv.textContent = buttonText;

    newComponent.appendChild(textDiv);

    newComponent.addEventListener('click', function () {
        selectComponent(compId);
    });

    // Create a div element for the drag icon
    var dragIcon = document.createElement('div');
    dragIcon.className = 'drag-icon';
    newComponent.appendChild(dragIcon);

    // Create a div element for the drag icon
    var panelIcon = document.createElement('div');
    panelIcon.className = 'panel-icon';
    panelIcon.id = "panel-icon-id";
    newComponent.appendChild(panelIcon);

    // Add onclick event to the panel icon
    panelIcon.addEventListener('click', function () {
        showModal(compId, buttonText, componentType);
    });

    // Append the new button to the canvas container
    document.getElementById('canvas-container').appendChild(newComponent);

    if (!desPageData.Comps) {
        desPageData.Comps = [];
    }

    // Update desPageData object
    desPageData.Comps.push({
        Id: compId, // Assign the unique ID
        DesCompType: componentType,
        PosX: newComponent.style.left,
        PosY: newComponent.style.top,
        Width: newComponent.style.width,
        Height: newComponent.style.height,
        TypeInfo: 'QLite.DesignComponents.' + componentType,
        ButtonText: buttonText,
    });
    //dragAndDrop('resize-drag');
}

// Event listener for segmentButton
document.getElementById('segmentButton').addEventListener('click', function () {
    createButton('Segment Button', 'DesCompDataSegment', 'button');
});

// Event listener for serviceButton
document.getElementById('serviceButton').addEventListener('click', function () {
    createButton('Service Button', 'DesCompDataServiceButton', 'button');
});

// Event listener for stepButton
document.getElementById('stepButton').addEventListener('click', function () {
    createButton('Step Button', 'DesCompDataWfButton', 'button');
});

// Event listener for languageButton
document.getElementById('languageButton').addEventListener('click', function () {
    createButton('Language Button', 'DesCompDataLang', 'button');
});

// Event listener for textButton
document.getElementById('textButton').addEventListener('click', function () {
    createButton('Text Component', 'DesCompDataText', 'div');
});

// Event listener for videoButton
document.getElementById('videoButton').addEventListener('click', function () {
    createButton('HTML Component', 'DesCompDataGenericHtml', 'div');
});

// Event listener for frameButton
document.getElementById('frameButton').addEventListener('click', function () {
    createButton('Frame Component', 'DesCompDataFrame', 'button');
});

function selectComponent(compId) {

    if (compId) {
        var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
        if (compIndex !== -1) {

            // Populate input fields if the component exists
            document.getElementById('posXInput').value = desPageData.Comps[compIndex].PosX;
            document.getElementById('posYInput').value = desPageData.Comps[compIndex].PosY;
            document.getElementById('widthInput').value = desPageData.Comps[compIndex].Width;
            document.getElementById('heightInput').value = desPageData.Comps[compIndex].Height;
            document.getElementById('selectedComp').value = desPageData.Comps[compIndex].ButtonText;

            $('#compID').val(compId);


        }
    }
}

//#endregion


//#region API Methods


// Function to save the design
document.getElementById('saveButton').addEventListener('click', function () {

    // Show page loading
    KTApp.showPageLoading();

    // Parse the query string of the current URL
    var queryString = window.location.search;
    var urlParams = new URLSearchParams(queryString);

    // Get the DesignID parameter from the query string
    var designID = urlParams.get('DesignID');

    // Check if DesignID is not null
    if (designID) {
        html2canvas(document.querySelector("#canvas-container")).then(canvas => {

            var designImage = canvas.toDataURL();
            var desPageDataJson = JSON.stringify(desPageData);

            // Perform your AJAX request with the DesignID included
            $.ajax({
                url: '/Designer/SaveDesign/' + designID,
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ desPageDataJson: desPageDataJson, DesignImage: designImage }),
                success: function (response) {
                    KTApp.hidePageLoading();
                    Swal.fire({
                        icon: 'success',
                        title: 'Design Saved',
                        showConfirmButton: false,
                        timer: 2000
                    });
                },
                error: function (xhr, status, error) {
                    KTApp.hidePageLoading();
                    Swal.fire({
                        icon: 'warning',
                        title: 'Error',
                        showConfirmButton: false,
                        timer: 2000
                    });
                }
            });

        });


    } else {
        KTApp.hidePageLoading();
        console.error('DesignID not found in URL');
        Swal.fire({
            icon: 'warning',
            title: 'Error',
            showConfirmButton: false,
            timer: 2000
        });
    }
});


// Function to populate Frame options
function getDesignByID(DesignID, compId) {

    // Show page loading
    KTApp.showPageLoading();
    // Code to execute after 0.5 seconds
    $.ajax({
        url: 'Designer/GetDesignImageByID/' + DesignID,
        method: 'GET',
        success: function (response) {
            $('#' + compId).css('background-image', 'url(' + response + ')');
            // Hide loading indicator
            KTApp.hidePageLoading();
            // loadingEl.remove();
        },
        error: function (xhr, status, error) {
            console.error('Error fetching design :', error);
            // Hide loading indicator
            KTApp.hidePageLoading();
            //loadingEl.remove();
        }
    });
}

// Function to populate Frame options
function populateFrameOptions(frameID) {
    $.ajax({
        url: 'Designer/GetDesignList',
        method: 'GET',
        success: function (response) {
            // Clear existing options
            $('#frameID').empty().append($('<option>', {
                value: '00000000-0000-0000-0000-000000000000',
                text: 'None'
            }));
            // Append new options
            $.each(response, function (index, design) {
                $('#frameID').append($('<option>', {
                    value: design.oid,
                    text: design.name,
                }));
            });

            // Set the selected value after the options have been populated
            $('#frameID').val(frameID);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching design list:', error);
        }
    });
}

// Function to populate segment options
function populateSegmentOptions(segmentID) {
    $.ajax({
        url: 'Designer/GetSegmentList',
        method: 'GET',
        success: function (response) {
            // Clear existing options
            $('#selectSegmentID').empty().append($('<option>', {
                value: '00000000-0000-0000-0000-000000000000',
                text: 'None'
            }));
            // Append new options
            $.each(response, function (index, segment) {
                $('#selectSegmentID').append($('<option>', {
                    value: segment.oid,
                    text: segment.name
                }));
            });

            // Set the selected value after the options have been populated
            $('#selectSegmentID').val(segmentID);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching segment list:', error);
        }
    });
}


// Function to populate service options
function populateServiceOptions(serviceTypeOid) {
    $.ajax({
        url: 'Designer/GetServiceList',
        method: 'GET',
        success: function (response) {
            // Clear existing options
            $('#selectServiceID').empty().append($('<option>', {
                value: '00000000-0000-0000-0000-000000000000',
                text: 'None'
            }));

            // Append new options
            $.each(response, function (index, service) {
                $('#selectServiceID').append($('<option>', {
                    value: service.oid,
                    text: service.name
                }));
            });

            // Set the selected value after the options have been populated
            $('#selectServiceID').val(serviceTypeOid);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching service list:', error);
        }
    });
}
// Function to populate service options
function populateLanguageOptions(languageOid) {
    $.ajax({
        url: 'Designer/GetLanguageList',
        method: 'GET',
        success: function (response) {
            // Clear existing options
            $('#selectLanguageID').empty().append($('<option>', {
                value: '00000000-0000-0000-0000-000000000000',
                text: 'None'
            }));

            // Append new options
            $.each(response, function (index, service) {
                $('#selectLanguageID').append($('<option>', {
                    value: service.oid,
                    text: service.name
                }));
            });

            // Set the selected value after the options have been populated
            $('#selectLanguageID').val(languageOid);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching service list:', error);
        }
    });
}

//#endregion

//#region Modal Methods



const componentConfigurations = {
    'DesCompDataServiceButton': {
        modalTitle: 'Edit Service Button',
        fieldsToShow: ['#BtnTextField', '#selectServiceIDField', '#cssCustomField', '#localizedField', '#bounceField', '#ImageField', '#compIdField'],
        additionalActions: populateServiceOptions
    },
    'DesCompDataSegment': {
        modalTitle: 'Edit Segment Button',
        fieldsToShow: ['#BtnTextField', '#selectSegmentIDField', '#cssCustomField', '#localizedField', '#bounceField', '#ImageField', '#compIdField'],
        additionalActions: populateSegmentOptions
    },
    'DesCompDataGenericHtml': {
        modalTitle: 'Edit HTML Component',
        fieldsToShow: ['#HtmlTypeField', '#compIdField'],
    },

    'DesCompDataFrame': {
        modalTitle: 'Edit Frame Component',
        fieldsToShow: ['#BtnTextField', '#cssCustomField', '#frameIDField', '#slideAnimationField', '#compIdField'],
        additionalActions: populateFrameOptions

    },

    'DesCompDataText': {
        modalTitle: 'Edit Text Component',
        fieldsToShow: ['#previewTextField', '#cssCustomField', '#textTypeField', '#localizedField', '#ctxIndexField', '#compIdField'],
    },

    'DesCompDataLang': {
        modalTitle: 'Edit Language Component',
        fieldsToShow: ['#BtnTextField', '#cssCustomField', '#languageIDField', '#selectLanguageID', '#ImageField', '#compIdField'],
        additionalActions: populateLanguageOptions

    },

    'DesCompDataWfButton': {
        modalTitle: 'Edit Step Component',
        fieldsToShow: ['#BtnTextField', '#cssCustomField', '#ImageField', '#compIdField'],
    },


    // Add other component configurations here...
};
// Function to handle file upload
function handleImageUpload(event) {
    var file = event.target.files[0];
    var reader = new FileReader();
    reader.onload = function (event) {
        var imageUrl = event.target.result;

        var compId = $('#compID').val();

        var component = desPageData.Comps.find(comp => comp.Id === compId);

        var element = document.getElementById(compId);
        // Set the background image URL 
        element.style.backgroundImage = 'url(' + imageUrl + ')';
        // Apply additional styles for background image 
        element.style.backgroundSize = 'cover';
        element.style.backgroundPosition = 'center';
        element.style.backgroundRepeat = 'no-repeat';
        element.style.border = '1px dotted black';
        // Update component data 
        component.BgImageUrl = imageUrl;

    };
    reader.readAsDataURL(file);

}



function showModal(compId, buttonText, componentType) {
    const componentConfiguration = componentConfigurations[componentType];

    if (!componentConfiguration) {
        console.error('Unsupported component type:', componentType);
        return;
    }

    const component = desPageData.Comps.find(comp => comp.Id === compId);

    // Check if the component is found
    if (component) {

        // Populate modal fields common to all components
        $('#buttonText').val(buttonText);
        $('#compID').val(compId);
        $('#componentModalLabel').text(componentConfiguration.modalTitle);
        $('#cssCustom').val(component.CustomCss);
        var id = '';

        // Hide all fields by default
        $('.modal-body .mb-3').hide();

        // Show necessary fields
        componentConfiguration.fieldsToShow.forEach(field => $(field).show());

        if (componentType === "DesCompDataServiceButton") {

            id = component.ServiceTypeOid;

            $('#bounce').prop('checked', component.Bounce);
        }
        else if (componentType === "DesCompDataSegment") {
            id = component.SegmentID;


            $('#localized').prop('checked', component.Localized);
            $('#bounce').prop('checked', component.Bounce);
        }

        else if (componentType === "DesCompDataText") {

            $('#slideAnimation').prop('checked', component.SlideAnimation);
            $('#ctxIndex').val(component.CtxIndex);
            $('#textType').val(component.InfoType);

            switch (component.InfoType) {
                case 0:
                case '0':
                    document.getElementById('previewText').value = 'T';
                    var compTextElement = document.getElementById(compId + '_text');

                    compTextElement.textContent = 'T';

                    component.Text = 'T';
                    break;
                case 1:
                case '1':
                    document.getElementById('previewText').value = '963';
                    component.Text = '963';

                    var compTextElement = document.getElementById(compId + '_text');
                    compTextElement.textContent = '963';

                    break;
                case 2:
                case '2':
                    document.getElementById('previewText').value = '27'
                    var compTextElement = document.getElementById(compId + '_text');
                    compTextElement.textContent = '27';

                    component.Text = '27';
                    break;
                case 3:
                case '3':
                    document.getElementById('previewText').value = 'Test Service';
                    var compTextElement = document.getElementById(compId + '_text');
                    compTextElement.textContent = 'Test Service';

                    component.Text = 'Test Service';
                    break;
                case 4:
                case '4':
                    document.getElementById('previewText').value = 'Test Segment';
                    var compTextElement = document.getElementById(compId + '_text');
                    compTextElement.textContent = 'Test Segment';

                    component.Text = 'Test Segment';
                    break;
                default:

                    break;
            }


        }
        else if (componentType === 'DesCompDataGenericHtml') {
            $('#youtubeVideoURL').val(component.YoutubeUrl);

            $('#localVideoURL').val(component.LocalUrl);

            $('#HtmlType').val(component.GenCompType);

            switch (component.GenCompType) {
                case 0:
                case '0':
                    $('#ImageField, #cssCustomField').show();
                    break;
                case 1:
                case '1':
                    $('#localVField, #cssCustomField').show();
                    break;
                case 2:
                case '2':
                    $('#youtubeField, #cssCustomField').show();
                    break;
                case 3:
                case '3':
                    $('#BtnTextField, #cssCustomField').show();
                    break;

                case 4:
                case '4':
                    $('#cssCustomField').show();

                case 5:
                case '5':
                    $('#cssCustomField').show();


                default:
                    // Handle default case
                    break;
            }



        }
        else if (componentType === 'DesCompDataFrame') {

            id = component.DesignId;

            $('#slideAnimation').prop('checked', component.SlideAnimation);

        }
        else if (componentType === 'DesCompDataLang') {

            id = component.LangID;

        }



        // Perform additional actions if needed
        if (componentConfiguration.additionalActions) {
            componentConfiguration.additionalActions(id);
        }

        // Show the modal
        $('#componentModal').modal('show');

    }
    else {
        Swal.fire({
            title: 'Component not found',
            text: 'Please refresh the page',
            icon: 'warning',
        })

    }
}
// Handle clone button click

$('#cloneButton').click(function () {
    var compId = $('#compID').val();
    var originalComp = desPageData.Comps.find(comp => comp.Id == compId);

    // Clone the original component, except for its Id
    var clonedComp = {};
    for (var key in originalComp) {
        if (originalComp.hasOwnProperty(key) && key !== 'Id') {
            clonedComp[key] = originalComp[key];
        }
    }

    var newCompId = 'comp_' + Date.now(); // Generate a unique ID for the new component
    var newPosX = '30px';
    var newPosY = '30px';
    // Create a new div element for the cloned component
    var newComponent = document.createElement('div');
    newComponent.id = newCompId;
    newComponent.className = 'resize-drag';
    newComponent.style.cssText = originalComp.CustomCss; // Copying Custom CSS
    newComponent.style.width = clonedComp.Width;
    newComponent.style.height = clonedComp.Height;
    newComponent.style.border = '1px dotted black';
    newComponent.style.transform = `translate(${newPosX}, ${newPosY})`;
    newComponent.setAttribute('data-x', newPosX);
    newComponent.setAttribute('data-y', newPosY);
    newComponent.setAttribute('data-comp-id', newCompId);


    // Create a div element for the drag icon
    var dragIcon = document.createElement('div');
    dragIcon.className = 'drag-icon';
    newComponent.appendChild(dragIcon);

    // Create a div element for the drag icon
    var panelIcon = document.createElement('div');
    panelIcon.className = 'panel-icon';
    panelIcon.id = "panel-icon-id";
    newComponent.appendChild(panelIcon);

    // Add onclick event to the panel icon
    panelIcon.addEventListener('click', function () {
        showModal(compId, clonedComp.ButtonText, clonedComp.DesCompType);
    });

    // Optional: If the original component has text, clone it
    if (clonedComp.ButtonText) {
        var textDiv = document.createElement('div');
        textDiv.id = newCompId + '_text';
        textDiv.textContent = clonedComp.ButtonText;
        newComponent.appendChild(textDiv);
    }

    // Add the cloned component to the canvas or desired container
    document.getElementById('canvas-container').appendChild(newComponent);

    // Add event listeners or other initialization logic as needed
    // For example, adding a click event listener to select the component
    newComponent.addEventListener('click', function () {
        selectComponent(newCompId);
    });

    // Update your components list with the new component data
    // Assigning new Id, PosX, and PosY while keeping the rest of the properties the same
    desPageData.Comps.push({
        ...clonedComp, // Spread operator to include all cloned properties
        Id: newCompId, // Assign the new unique ID
        PosX: newPosX, // Assign new X position
        PosY: newPosY, // Assign new Y position
    });
});

// Handle delete button click
$('#deleteButton').click(function () {
    var compId = $('#compID').val();

    Swal.fire({
        title: 'Are you sure?',
        text: 'You will not be able to recover this component!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            // Remove the component from desPageData.Comps
            desPageData.Comps = desPageData.Comps.filter(comp => comp.Id !== compId);
            // Remove the HTML element associated with the component
            $('#' + compId).remove();
            Swal.fire(
                'Deleted!',
                'The component has been deleted.',
                'success'
            );

            // Close the modal
            $('#componentModal').modal('hide');
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Your component is safe :)',
                'error'
            );
        }
    });
});


$('#saveComponentButton').click(function () {
    var compId = $('#compID').val();
    var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);

    if (compIndex !== -1) {

        // If elements with IDs compId_text or compId_frame exist, delete them
        $('#' + compId + '_text').remove();
        $('#' + compId + '_frame').remove();


        var comp = desPageData.Comps[compIndex];

        // Using object to map input IDs to comp properties
        var inputsToCompProps = {
            'buttonText': 'Text',
            'buttonText': 'ButtonText',
            'selectSegmentID': 'SegmentID',
            'selectServiceID': 'ServiceTypeOid',
            'frameID': 'DesignId',
            'cssCustom': 'CustomCss',
            'ctxIndex': 'CtxIndex',
            'localVideoURL': 'LocalUrl',
            'youtubeVideoURL': 'YoutubeUrl',
            'selectLanguageID': 'LangID',
            'textType': 'InfoType',
            'HtmlType': 'GenCompType',

        };

        Object.entries(inputsToCompProps).forEach(([inputId, propName]) => {
            var value = $('#' + inputId).val();
            if (value !== undefined) comp[propName] = value;
        });

        // Update boolean properties
        ['bounce', 'slideAnimation'].forEach(propId => {
            comp[propId.charAt(0).toUpperCase() + propId.slice(1)] = $('#' + propId).prop('checked');
        });

        // Specific updates based on DesCompType
        if (comp.DesCompType === 'DesCompDataFrame') {
            getDesignByID(comp.DesignId, comp.Id);
        }

        if (comp.DesCompType === 'DesCompDataGenericHtml' && (comp.GenCompType === 2 || comp.GenCompType === '2')) {

            $('#' + compId).css('background-image', 'none');

            var frameId = compId + '_frame';
            var $frame = $('#' + frameId);
            var videoId = comp.YoutubeUrl.split('/').pop().split('?')[0];

            // Check if the element exists
            if ($frame.length === 0) {
                // Element does not exist, proceed to add it
                var iframeHtml =
                    '<iframe id="' + frameId + '" style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); width: 95%; height: 95%; border: none;" ' +
                    ' src="' + comp.YoutubeUrl + '?controls=0&mute=1&showinfo=0&rel=0&autoplay=1&loop=1&playlist=' + videoId + '"' +
                    '></iframe>';

                // Append the new iframeHtml to the element with the id compId
                $('#' + compId).append(iframeHtml);
            } else if (comp.DesCompType === 'DesCompDataGenericHtml' && (comp.GenCompType === 2 || comp.GenCompType === '2')) {
                // If the frame already exists and meets certain conditions, update its 'src' attribute
                $frame.attr('src', comp.YoutubeUrl + '?controls=0&mute=1&showinfo=0&rel=0&autoplay=1&loop=1&playlist=' + videoId);
            }
        }

        else {

            var textId = compId + '_text';
            var $text = $('#' + textId);

            if ($text.length === 0) {
                var textHtml = '<div id="' + textId + '">' + comp.ButtonText + '</div> ';
                // Append the new iframeHtml to the element with the id compId
                $('#' + compId).append(textHtml);
            }
            else {
                $('#' + compId + '_text').text(comp.ButtonText);

            }
        }
        // Apply custom CSS if present
        if (comp.CustomCss) {
            var cssRules = comp.CustomCss.split(';');
            cssRules.forEach(rule => {
                var [property, value] = rule.split(':');
                if (property && value) $('#' + compId).css(property.trim(), value.trim());
            });
        }
    }

    $('#componentModal').modal('hide');
});





document.getElementById('textType').addEventListener('change', function () {
    // Get the selected option
    var selectedOption = this.value;
    var compId = $('#compID').val();

    // Find the component in desPageData.Comps
    var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
    if (compIndex === -1) {
        console.error('Component not found in desPageData.Comps');
        return;
    }
    var comp = desPageData.Comps[compIndex];

    // Update the preview text based on the selected option
    switch (selectedOption) {
        case '0':
            document.getElementById('previewText').value = 'T';
            var compTextElement = document.getElementById(compId + '_text');

            compTextElement.textContent = 'T';

            comp.Text = 'T';
            comp.TicketInfoType = selectedOption;
            break;
        case '1':
            document.getElementById('previewText').value = '963';
            comp.Text = '963';

            var compTextElement = document.getElementById(compId + '_text');
            compTextElement.textContent = '963';

            comp.TicketInfoType = selectedOption;
            break;
        case '2':
            document.getElementById('previewText').value = '27'
            var compTextElement = document.getElementById(compId + '_text');
            compTextElement.textContent = '27';

            comp.Text = '27';
            comp.TicketInfoType = selectedOption;
            break;
        case '3':
            document.getElementById('previewText').value = 'Test Service';
            var compTextElement = document.getElementById(compId + '_text');
            compTextElement.textContent = 'Test Service';

            comp.Text = 'Test Service';
            comp.TicketInfoType = selectedOption;
            break;
        case '4':
            document.getElementById('previewText').value = 'Test Segment';
            var compTextElement = document.getElementById(compId + '_text');
            compTextElement.textContent = 'Test Segment';

            comp.Text = 'Test Segment';
            comp.TicketInfoType = selectedOption;
            break;
        default:
            // Handle default case (optional)
            document.getElementById('previewText').value = '';
            break;
    }
});

document.getElementById('HtmlType').addEventListener('change', function () {
    // Get the selected option
    var selectedOption = this.value;

    var compId = $('#compID').val();

    // Find the component in desPageData.Comps
    var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
    if (compIndex === -1) {
        console.error('Component not found in desPageData.Comps');
        return;
    }
    var comp = desPageData.Comps[compIndex];

    // Hide all fields by default
    $('.modal-body .mb-3').hide();

    $('#HtmlTypeField').show();


    comp.GenCompType = selectedOption;


    // Update the preview text based on the selected option
    switch (selectedOption) {
        case '0':
            $('#ImageField, #cssCustomField').show();

            break;
        case '1':
            $('#localVField, #cssCustomField').show();
            break;
        case '2':
            $('#youtubeField, #cssCustomField').show();
            break;
        case '3':
            $('#BtnTextField, #cssCustomField').show();
            break;

        case '4':
            $('#cssCustomField').show();

            var compTextElement = document.getElementById(compId + '_text');

            compTextElement.textContent = Date.now();
            $('#buttonText').val(Date.now());

        case '5':
            $('#cssCustomField').show();

            break;
        default:

            break;
    }
});


// Add click event listener to the button
document.getElementById('popupButton').addEventListener('click', function () {
    // Show Swal (SweetAlert) popup
    Swal.fire({
        title: 'CSS Properties',
        html: `
            <p>CSS (Cascading Style Sheets) is a styling language used for describing the presentation of a document written in HTML.</p>
            <p>It consists of a set of rules applied to HTML elements to control their appearance.</p>
           
            <h4>Examples of Inline CSS Styles:</h4>
            <p><h2>Buttons : </h2></p>
            <p>color: #ff6600; </p>
            <p>background-color: #0A2351;</p>
            <p>font-size: 25px;</p>
            <p> font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;</p>
            <p>font-weight: bold;</p>
            <p>padding: 10px;</p>
            <p>margin: 20px;</p>
            <p>border: 5px solid black;</p>
            <p>text-align: center;</p>
            <p>display: block;</p>
            <p>box-shadow: 0 4px 8px rgba(0, 0, 0.5, 1);</p>

            <p><h2>Texts : </h2></p>
            <p>font-family: 'Arial Black', sans-serif; </p>
            <p>font-size: 50px;</p>
            <p>color: #ff6600;</p>
            <p> text-shadow: 2px 2px 4px #000000;</p>
            <p>text-align: center;</p>
            <p>display: block;</p>

            <p><h2>Layouts and Shapes : </h2></p>
            <p>border-radius: 0 0 20px 20px;</p>
            <p>padding: 0 60px;</p>
            <p>background: #0A2351;</p>
           
          
            <h4>Common CSS Properties:</h4>
            <ul>
                <li><b>color:</b> Specifies the text color. Example: color: black;</li>
                <li><b>font-size:</b> Sets the size of the font. Example: font-size: 16px;</li>
                <li><b>background-color:</b> Defines the background color of an element. Example: background-color: #f0f0f0;</li>
                <li><b>margin:</b> Controls the space around elements. Example: margin: 10px;</li>
                <li><b>padding:</b> Specifies the space between the content and the border. Example: padding: 5px;</li>
                <li><b>border:</b> Sets the border properties. Example: border: 1px solid #000;</li>
                <li><b>text-align:</b> Aligns the text horizontally. Example: text-align: center;</li>
                <li><b>display:</b> Defines how an element is displayed. Example: display: block;</li>
                <li><b>font-family:</b> Specifies the font family for text. Example: font-family: Arial, sans-serif;</li>
                <li><b>font-weight:</b> Sets the boldness of the font. Example: font-weight: bold;</li>
                <li><b>text-decoration:</b> Adds decorations to text. Example: text-decoration: underline;</li>
                <li><b>border-radius:</b> Defines the curvature of border corners. Example: border-radius: 5px;</li>
                <li><b>box-shadow:</b> Adds shadows to elements. Example: box-shadow: 2px 2px 5px #888888;</li>
            </ul>
            <p>For more CSS properties, you can refer to <a href="https://www.w3schools.com/cssref/index.php" target="_blank">W3Schools CSS Reference</a>.</p>
        `,
        icon: 'info',
        confirmButtonText: 'OK',
        customClass: {
            popup: 'custom-popup-class'
        }
    });
});


// Add click event listener to the button
document.getElementById('YtTutorialButton').addEventListener('click', function () {
    // Show Swal (SweetAlert) popup
    Swal.fire({
        title: 'How to Get the Embedded YouTube Video Link',
        html: `
            <div>
                <p>Follow these steps to get the embedded YouTube video link:</p>
                <ol>
                    <li>Open the YouTube video you want to embed in a new tab.</li>
                    <li>Click the "Share" button located below the video.</li>
                    <li>Select the "Embed" option from the menu.</li>
                    <li>Copy the src link provided in the embed code. For example: <br> 
                        <code>https://www.youtube.com/embed/VIDEO_ID</code></li>
                    <li>Paste the link into the input field above.</li>
                </ol>
                <p><strong>Note:</strong> The src link typically starts with <code>https://www.youtube.com/embed/</code> followed by the video ID.</p>
            </div>`,
        icon: 'info',
        confirmButtonText: 'OK',
        customClass: {
            popup: 'custom-popup-class'
        }
    });
});
//#endregion


//#region ToolBar Methods


document.getElementById('widthInput').addEventListener('change', function () {
    var compId = $('#compID').val();
    var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
    if (compIndex === -1) {
        console.error('Component not found in desPageData.Comps');
        return;
    }
    var comp = desPageData.Comps[compIndex];

    // Get the changed width value from the input field
    var newWidth = $(this).val();

    // Update the component's width
    comp.Width = newWidth + (newWidth.includes('px') ? '' : 'px');

    // Get the component element
    var element = $('#' + compId)[0];

    // Set the width of the element
    element.style.width = newWidth + (newWidth.includes('px') ? '' : 'px');

    // Set the data-x attribute of the element
    // element.setAttribute('data-x', newWidth);
});

document.getElementById('heightInput').addEventListener('change', function () {

    var compId = $('#compID').val();
    var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
    if (compIndex === -1) {
        console.error('Component not found in desPageData.Comps');
        return;
    }
    var comp = desPageData.Comps[compIndex];

    // Get the changed width value from the input field
    var newHeight = $(this).val();

    // Update the component's width
    comp.Height = newHeight + (newHeight.includes('px') ? '' : 'px');;

    // Get the component element
    var element = $('#' + compId)[0];

    // Set the width of the element
    element.style.height = newHeight + (newHeight.includes('px') ? '' : 'px');

    // Set the data-x attribute of the element
    //element.setAttribute('data-y', newHeight);
});
//#endregion
