﻿
dragAndDrop('resize-drag');

const loadingEl = document.createElement("div");
document.body.prepend(loadingEl);
loadingEl.classList.add("page-loader");
loadingEl.classList.add("flex-column");
loadingEl.innerHTML = `
        <span class="spinner-border text-primary" role="status"></span>
        <span class="text-muted fs-6 fw-semibold mt-5">Saving Changes Please wait...</span>
    `;



/////////////////  CANVAS    //////////////////////////

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

// Function to update background color in the desPageData object
function updateBackgroundColor() {
    var colorPicker = document.getElementById('backgroundColor');
    var selectedColor = colorPicker.value;
    document.getElementById('canvas-container').style.backgroundColor = selectedColor;

    // Update desPageData property
    desPageData.BackGroundColor = selectedColor;
}

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

// Event listener for color picker change
document.getElementById('backgroundColor').addEventListener('change', updateBackgroundColor);

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




/////////////////  Buttons    //////////////////////////



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
    dragAndDrop('resize-drag');
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



/////////////////   API Methods  //////////////////////////

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

        var node = document.getElementById('kt_content_container');
        var designImage;

        domtoimage.toPng(node)
            .then(function (dataUrl) {
                // Store the generated image
                designImage = dataUrl;

                var desPageDataJson = JSON.stringify(desPageData);

                // Perform your AJAX request with the DesignID included
                $.ajax({
                    url: '/Designer/SaveDesign/' + designID,
                    method: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ desPageDataJson: desPageDataJson, DesignImage: designImage }),
                    success: function (response) {
                        KTApp.hidePageLoading();
                        //loadingEl.remove();
                        Swal.fire({
                            icon: 'success',
                            title: 'Design Saved',
                            showConfirmButton: false,
                            timer: 2000
                        });
                    },
                    error: function (xhr, status, error) {
                        KTApp.hidePageLoading();
                        //loadingEl.remove();
                        Swal.fire({
                            icon: 'warning',
                            title: 'Error',
                            showConfirmButton: false,
                            timer: 2000
                        });
                    }
                });
            })
            .catch(function (error) {
                KTApp.hidePageLoading();
                //loadingEl.remove();
                console.error('Oops, something went wrong!', error);
            });
    } else {
        KTApp.hidePageLoading();
        // loadingEl.remove();
        console.error('DesignID not found in URL');
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


/////////////////   Modal Methods  //////////////////////////


const componentConfigurations = {
    'DesCompDataServiceButton': {
        modalTitle: 'Edit Service Button',
        fieldsToShow: ['#BtnTextField', '#selectServiceIDField', '#cssCustomField', '#localizedField', '#bounceField', '#compIdField'],
        additionalActions: populateServiceOptions
    },
    'DesCompDataSegment': {
        modalTitle: 'Edit Segment Button',
        fieldsToShow: ['#BtnTextField', '#selectSegmentIDField', '#cssCustomField', '#localizedField', '#bounceField', '#compIdField'],
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
        fieldsToShow: ['#BtnTextField', '#cssCustomField', '#languageIDField', '#compIdField'],
    },

    'DesCompDataWfButton': {
        modalTitle: 'Edit Step Component',
        fieldsToShow: ['#BtnTextField', '#cssCustomField', '#compIdField'],
    },


    // Add other component configurations here...
};
var imageBase64 = '';



function showModal(compId, buttonText, componentType) {
    const componentConfiguration = componentConfigurations[componentType];

    if (!componentConfiguration) {
        console.error('Unsupported component type:', componentType);
        return;
    }

    var component = desPageData.Comps.find(comp => comp.Id === compId);

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

            $('#localized').prop('checked', component.Localized);
            $('#bounce').prop('checked', component.Bounce);
        }
        else if (componentType === "DesCompDataSegment") {
            id = component.SegmentID;


            $('#localized').prop('checked', component.Localized);
            $('#bounce').prop('checked', component.Bounce);
        }

        else if (componentType === "DesCompDataText") {

            $('#slideAnimation').prop('checked', component.SlideAnimation);
            $('#dataAware').prop('checked', component.DataAware);
            $('#ctxIndex').val(component.CtxIndex);
            $('#localized').prop('checked', component.Localized);
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

            // id = comp.LanguageName;

        }

     

        // Perform additional actions if needed
        if (componentConfiguration.additionalActions) {
            componentConfiguration.additionalActions(id);
        }

        function handleImageUpload(event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onload = function (event) {
                var imageUrl = event.target.result;
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
        // Remove existing event listener for the image uploader
        document.getElementById('componentImageUpload').removeEventListener('change', handleImageUpload);

        // Function to handle file upload


        // Add event listener for the image uploader
        document.getElementById('componentImageUpload').addEventListener('change', handleImageUpload);


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
            'selectLanguageID': 'LanguageName',
            'textType': 'InfoType',
            'HtmlType': 'GenCompType'
        };

        Object.entries(inputsToCompProps).forEach(([inputId, propName]) => {
            var value = $('#' + inputId).val();
            if (value !== undefined) comp[propName] = value;
        });

        // Update boolean properties
        ['localized', 'bounce', 'dataAware', 'slideAnimation'].forEach(propId => {
            comp[propId.charAt(0).toUpperCase() + propId.slice(1)] = $('#' + propId).prop('checked');
        });

        // Specific updates based on DesCompType
        if (comp.DesCompType === 'DesCompDataFrame') {
            getDesignByID(comp.DesignId, comp.Id);
        }
        if (comp.DesCompType !== 'DesCompDataText') {
            $('#' + compId + '_text').text(comp.ButtonText);
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

            break;
        default:
           
            break;
    }
});

