document.addEventListener("DOMContentLoaded", function () {

    /////////////////  CANVAS    //////////////////////////

    // Function to update canvas size in the desPageData object
    function updateCanvasSize() {
        var widthSelect = document.getElementById('widthSelect');
        var heightSelect = document.getElementById('heightSelect');
        var selectedWidth = widthSelect.value + 'px';
        var selectedHeight = heightSelect.value + 'px';
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






    /////////////////  Buttons    //////////////////////////


    document.getElementById('segmentButton').addEventListener('click', function () {
        console.log(desPageData);

        // Create a new segment button element
        var newSegmentButton = document.createElement('button');
        var compId = 'comp_' + Date.now(); // Generate a unique ID for the component
        newSegmentButton.id = compId; // Set the id attribute
        newSegmentButton.textContent = 'Segment Button';
        newSegmentButton.style.width = '100px'; // Set default width
        newSegmentButton.style.height = '100px'; // Set default height
        newSegmentButton.className = 'resize-drag';

        newSegmentButton.setAttribute('data-comp-id', compId);

        // Create a div element for the drag icon
        var dragIcon = document.createElement('div');
        dragIcon.className = 'drag-icon';
        newSegmentButton.appendChild(dragIcon);

        // Create a div element for the drag icon
        var panelIcon = document.createElement('div');
        panelIcon.className = 'panel-icon';
        panelIcon.id = "panel-icon-id";
        newSegmentButton.appendChild(panelIcon);

        // Add onclick event to the panel icon
        panelIcon.addEventListener('click', function () {
            showModal(compId);
        });

        // Append the new button to the canvas container
        document.getElementById('canvas-container').appendChild(newSegmentButton);



        if (!desPageData.Comps) {
            desPageData.Comps = [];
        }

        // Update desPageData object
        desPageData.Comps.push({
            Id: compId, // Assign the unique ID
            DesCompType: 'DesCompDataSegment',
            PosX: newSegmentButton.style.left,
            PosY: newSegmentButton.style.top,
            Width: newSegmentButton.style.width,
            Height: newSegmentButton.style.height,
            TypeInfo: 'QLite.DesignComponents.DesCompDataSegment',
            ButtonText:'Segment Button',
        });
        console.log(desPageData);
        dragAndDrop('resize-drag');

    });

   
  

    /////////////////   API Methods  //////////////////////////

    // Function to save the design
    document.getElementById('saveButton').addEventListener('click', function () {

        // Parse the query string of the current URL
        var queryString = window.location.search;
        var urlParams = new URLSearchParams(queryString);

        // Get the DesignID parameter from the query string
        var designID = urlParams.get('DesignID');

        // Check if DesignID is not null
        if (designID) {
            var desPageDataJson = JSON.stringify(desPageData);


            // Perform your AJAX request with the DesignID included
            $.ajax({
                url: '/Designer/SaveDesign/' + designID,
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ desPageDataJson: desPageDataJson }),
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Design Saved',
                        showConfirmButton: false,
                        timer: 2000
                    });
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Error',
                        showConfirmButton: false,
                        timer: 2000
                    });
                }
            });
        } else {
            console.error('DesignID not found in URL');
        }
    });

});
