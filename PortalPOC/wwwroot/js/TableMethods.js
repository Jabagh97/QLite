// Helper function to create DataTable buttons
function createDataTableButton(extend, text, titleAttr) {
    return {
        extend: extend,
        text: text,
        titleAttr: titleAttr
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
            title: 'New entry saved',
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
        timer: 5000
    }).then(function () {
        location.reload();
    });
}

// Main function to add DataTable buttons based on input buttonNames
function addButtons(...buttonNames) {
    let buttons = [];

    if (buttonNames.includes("importCsv")) {
        buttons.push({
            text: '<i class="fa-solid fa-file-csv"></i>Import CSV',
            action: function (e, dt, button, config) {
                showModal('csvModal', 'csvModalBody', 'csvForm', null, null);
            },
            className: 'editBtn'
        });
    }

    const dataTableButtons = {
        csv: createDataTableButton('csv', '<i class="fa fa-file-csv"></i>Export CSV', 'CSV'),
        copy: createDataTableButton('copy', '<i class="fa fa-file"></i>Copy', 'Copy'),
        excel: createDataTableButton('excel', '<i class="fa fa-file-excel"></i>Excel', 'Excel'),
        pdf: createDataTableButton('pdf', '<i class="fa fa-file-pdf"></i>PDF', 'PDF'),
        print: createDataTableButton('print', '<i class="fa fa-print"></i>Print', 'Print')
    };

    buttonNames.forEach(buttonName => {
        if (dataTableButtons[buttonName]) {
            buttons.push(dataTableButtons[buttonName]);
        }
    });

    return buttons;
}

// Function to initialize base modal button
function initializeBaseModalButton(viewModelName, buttonName) {
    let baseModalButton = {
        text: `<i class="fa fa-${buttonName.toLowerCase()}"></i>${buttonName.charAt(0).toUpperCase() + buttonName.slice(1)}`,
        className: buttonName.toLowerCase() + 'Btn'
    };
  
    if (buttonName === 'Add') {
        baseModalButton.action = function (e, dt, node, config) {
            showModal("kt_modal_3", "addModalBody", "createForm",viewModelName, buttonName);
        };
    } else if (buttonName === 'Edit') {
        baseModalButton.extend = 'selectedSingle';
        baseModalButton.action = function (e, dt, button, config) {
            var data = dt.row({ selected: true }).data();
            showModal("kt_modal_3_Edit", "editModalBody", "editForm",viewModelName, buttonName, { 'data': data });
        };
    } else if (buttonName === 'Delete') {
        baseModalButton.extend = 'selected';
        baseModalButton.text = '<i class="ki-outline ki-trash fs-3" style="position:relative; top:3px;"></i>Delete';
        baseModalButton.action = function (e, dt, button, config) {
            var oids = Array.from(dt.rows({ selected: true }).data()).map(obj => obj.oid);
            showDeleteConfirmation(viewModelName, oids);
        };
    }

    return baseModalButton;
}
// Helper function to show modals
function showModal(modalId, modalBodyId, formId, viewModel, buttonName, additionalData = {}) {
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