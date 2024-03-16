// Initialize a JavaScript object similar to DesPageData class
var desPageData = {
    Name: "",
    PosY: "",
    Width: "",
    Height: "",
    BgImageUrl: "",
    BackGroundColor: "",
    Comps: []
};


// Function to save the design
function saveDesign() {
    // Parse the query string of the current URL
    var queryString = window.location.search;
    var urlParams = new URLSearchParams(queryString);

    // Get the DesignID parameter from the query string
    var designID = urlParams.get('DesignID');

    // Check if DesignID is not null
    if (designID) {
        // Perform your AJAX request with the DesignID included
        $.ajax({
            url: '/Designer/SaveDesign/' + designID,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(desPageData),
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
}
