
//Init Table stuff
function initializeDataTable(modelName, columnDefinitions) {
    let dt;
    let table;

    function initializeDatatable() {
        dt = $('#table').DataTable({
            searchDelay: 500,
            //serverSide: true,
            //processing: true,
            //stateSave: true,
            paging: true,
            filter: true,
            dom: 'Bfrtilp',
            select: true,
            ajax: {
                url: '/GenericTable/GetData',
                type: "POST",
                data: { "modelName": modelName },
                dataType: "json",
                error: function (xhr, error, code) {
                    console.log(xhr, code);
                }
            },
            columns: columnDefinitions,
            columnDefs: [
                { targets: '_all', "defaultContent": "" },
                {
                    targets: '_all',
                    "render": function (data, type, row, meta) {
                        if (String(data).length <= 100) {
                            return data
                        }
                        else {
                            return data.substring(0, 30) + ".....";
                        }
                    }
                }
            ],
            lengthMenu: [
                [5, 10, 25, 50, 1000],
                ['5 rows', '10 rows', '25 rows', '50 rows', 'Show all']
            ],
            pageLength: 10,
            buttons: generateButtons(modelName)
        });

        table = dt.$;
    }

    // Double-click event listener on rows
    $('#table tbody').on('dblclick', 'tr', function () {
        var data = dt.row(this).data(); // Accessing DataTable instance (dt) here
        showEditModal("kt_modal_3_Edit", "editModalBody", "editForm", modelName, 'Edit', { 'data': data });
    });

    function generateButtons(modelName) {
        var buttons = [
            // Include export buttons here
            'csv', 'copy', 'excel', 'pdf', 'print'
        ];

        if (modelName !== 'KioskPkg' && !modelName.includes('KioskRequest')) {
            buttons.unshift('Add', 'Edit', 'Delete');
        }

        return buttons.map(function (button) {
            if (button === 'Add' || button === 'Edit' || button === 'Delete') {
                return initializeBaseModalButton(modelName, button);
            } else {
                return button;
            }
        });
    }

    // Function to initialize base modal button
    function initializeBaseModalButton(viewModelName, buttonName) {
        let baseModalButton = {
            text: `<i class="fa fa-${buttonName.toLowerCase()}"></i>${buttonName.charAt(0).toUpperCase() + buttonName.slice(1)}`,
            className: ''
        };

        if (buttonName === 'Add') {
            baseModalButton.action = function (e, dt, node, config) {
                showAddModal("kt_modal_3", "addModalBody", "createForm", viewModelName, buttonName);
            };
        } else if (buttonName === 'Edit') {
            baseModalButton.extend = 'selectedSingle';
            baseModalButton.action = function (e, dt, button, config) {
                var data = dt.row({ selected: true }).data();
                showEditModal("kt_modal_3_Edit", "editModalBody", "editForm", viewModelName, buttonName, { 'data': data });
            };
        } else if (buttonName === 'Delete') {
            baseModalButton.extend = 'selected';
            baseModalButton.text = '<i class="ki-outline ki-trash fs-3"></i>Delete';
            baseModalButton.action = function (e, dt, button, config) {
                var oids = Array.from(dt.rows({ selected: true }).data()).map(obj => obj.oid);
                showDeleteConfirmation(viewModelName, oids);
            };
        }

        return baseModalButton;
    }

    // ... (existing code for initializeBaseModalButton)

    return {
        init: initializeDatatable(),
    };
}



// Helper function to handle form submissions
function handleFormSubmission(viewModel, buttonName, formId) {
    var formDataObject = {};
    var formData = new FormData(document.getElementById(formId));

    formData.forEach(function (value, key) {
        formDataObject[key] = value;
    });

    formDataObject['modelType'] = viewModel;

    $.ajax({
        url: $("#" + formId).attr("action"),
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formDataObject),
        success: function (response) {
            handleFormSubmissionSuccess(response, viewModel, buttonName);
        },
        error: function (error) {
            handleFormSubmissionError(error);
        }
    });
}

// Helper function to handle form submission success
function handleFormSubmissionSuccess(response, viewModel, buttonName) {
    if (response.success === false) {
        handleFormSubmissionFailure(response);
    } else {
        $('#' + (buttonName.toLowerCase() === 'add' ? 'addModal' : 'kt_modal_3_Edit')).modal('hide');
        $('#table').DataTable().ajax.reload();
        Swal.fire({
            icon: 'success',
            title: 'Entry saved',
            showConfirmButton: false,
            timer: 1000
        });
    }

    KTApp.hidePageLoading();
}

// Helper function to handle form submission failure
function handleFormSubmissionFailure(response) {
    if (response.errors == null) {
        Swal.fire({
            icon: 'error',
            title: 'An unexpected error happened',
            showConfirmButton: false,
            timer: 2000
        });
    } else {
        var errorMessages = response.errors.join('<br>');
        $('#errorContainer').html(errorMessages);
    }
}

// Helper function to handle form submission error
function handleFormSubmissionError(error) {
    Swal.fire({
        icon: 'error',
        title: 'An unexpected error occurred: ' + error
    });

    KTApp.hidePageLoading();
}

// Helper function to show delete confirmation
function showDeleteConfirmation(viewModel, oids) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: 'Are you sure?',
        text: 'Are you sure you want to delete the selected items?',
        icon: 'warning',
        showCancelButton: true,
        cancelButtonText: 'Cancel',
        confirmButtonText: 'Delete',
        reverseButtons: true
    }).then((result) => {
        var url = `/GenericTable/Delete`;
        var payload = {
            modelType: viewModel,
            Oid: oids
        };

        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(payload),
                contentType: "application/json",
                success: function (response) {
                    handleDeleteConfirmationSuccess(response);
                },
                error: function (error) {
                    handleErrors(401);
                }
            });
        }
    });
}

// Helper function to handle delete confirmation success
function handleDeleteConfirmationSuccess(response) {
    $('#table').DataTable().ajax.reload();
    if (response.success === true) {
        Swal.fire({
            icon: 'success',
            title: 'Record deleted',
            showConfirmButton: false,
            timer: 1000
        });
    } else {
        handleErrors(401);
    }
}

// Helper function to handle errors
function handleErrors(status) {
    var title, text;
    if (status == 401) {
        title = 'Session timed out';
        text = 'Please login again';
    } else {
        title = 'Error';
        text = 'An unexpected error occurred';
    }

    Swal.fire({
        icon: 'warning',
        title: title,
        text: text,
        showConfirmButton: false,
        timer: 50000
    }).then(function () {
        location.reload();
    });
}





// Helper function to show modals
function showAddModal(modalId, modalBodyId, formId, viewModel, buttonName, additionalData = {}) {
    var url = `GenericTable/ShowPopup?modelName=${viewModel}&opType=${buttonName}`;
    $("#" + modalBodyId).load(url, additionalData, function (response, status, xhr) {
        if (status === "error") {
            handleErrors(xhr.status);
        } else {
            $('#' + modalId).modal('show');
            $("#" + modalBodyId).off('click', '#submit'); // Remove existing click event handler
            $("#" + modalBodyId).on('click', '#submit', function (event) {
                event.preventDefault();
                handleFormSubmission(viewModel, buttonName, formId);
            });
        }
    });
}
// Helper function to show modals
function showEditModal(modalId, modalBodyId, formId, viewModel, buttonName, additionalData = {}) {

    var url = `GenericTable/ShowPopup?modelName=${viewModel}&opType=${buttonName}`;

    // Add slide-out class to initiate animation
    $('#Wrapper').addClass('slide-out');

    // Load modal content
    $("#Wrapper").load(url, additionalData, function (response, status, xhr) {
        if (status === "error") {
            handleErrors(xhr.status);
        } else {
            // Wait for the slide-out animation to complete, then replace the content and slide in the new content
            setTimeout(function () {
                // Remove the slide-out class and add the slide-in class to slide in the new content
                $('#Wrapper').removeClass('slide-out').addClass('slide-in');

                // Attach event handler for form submission
                $('#Wrapper').off('click', '#submit'); // Remove existing click event handler
                $('#Wrapper').on('click', '#submit', function (event) {
                    event.preventDefault();
                    handleFormSubmission(viewModel, buttonName, formId);
                });
            }, 150); // Adjust the timeout value to match the duration of your slide-out animation
        }
    });
}




// Function to handle the "Back" action
function back() {

    location.reload(true);


}


// Function to handle file selection and display preview
function handleFileSelect(propertyName, fileInputId) {
    var fileInput = document.getElementById(fileInputId);
    var preview = document.getElementById(`${propertyName}Preview`);
    var hiddenInput = document.getElementById(`${propertyName}HiddenInput`);

    var file = fileInput.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            hiddenInput.value = e.target.result.split(',')[1];
            preview.style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
}

