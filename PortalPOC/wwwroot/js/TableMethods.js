

function addButtons(...buttonNames) {
   

    let buttons = [];

    if (buttonNames.includes("importCsv")) {
        buttons.push({
            text: '<i class="fa-solid fa-file-csv"></i>Import CSV',
            action: function (e, dt, button, config) {
                $("#csvModalBody").load("/Resources/CsvPopup",
                    function (response, status, xhr) {
                        if (status == "error") {
                            Swal.fire({
                                icon: 'warning',
                                title: 'Session timed out',
                                text: "Please login again",
                                showConfirmButton: false,
                                timer: 5000
                            }).then(function () {
                                location.reload();
                            });
                        }
                        else {
                            $('#csvModal').modal('show');
                        }
                    });
            },
            className: 'editBtn'
        });
    }

    if (buttonNames.includes('csv')) {
        buttons.push({
            extend: 'csv',
            text: '<i class="fa fa-file-csv"></i>Export CSV',
            titleAttr: 'CSV'
        });
    }

    if (buttonNames.includes('copy')) {
        buttons.push({
            extend: 'copy',
            text: '<i class="fa fa-file"></i>Copy',
            titleAttr: 'Copy'
        });
    }

    if (buttonNames.includes('excel')) {
        buttons.push({
            extend: 'excel',
            text: '<i class="fa fa-file-excel"></i>Excel',
            titleAttr: 'Excel'
        });
    }



    if (buttonNames.includes('pdf')) {
        buttons.push({
            extend: 'pdf',
            text: '<i class="fa fa-file-pdf"></i>PDF',
            titleAttr: 'PDF'
        });
    }

    if (buttonNames.includes('print')) {
        buttons.push({
            extend: 'print',
            text: '<i class="fa fa-print"></i>Print',
            exportOptions: {
                columns: ':visible'
            },
            customize: function (win) {
                var last = null;
                var current = null;
                var bod = [];

                var css = '@@page { size: landscape; }',
                    head = win.document.head || win.document.getElementsByTagName('head')[0],
                    style = win.document.createElement('style');

                style.type = 'text/css';
                style.media = 'print';

                if (style.styleSheet) {
                    style.styleSheet.cssText = css;
                } else {
                    style.appendChild(win.document.createTextNode(css));
                }

                head.appendChild(style);
            }
        });
    }




    return buttons;
}



function initializeBaseModalButton(viewModelName, buttonName) {
    let baseModalButton = {
        text: `<i class="fa fa-${buttonName.toLowerCase()}"></i>${buttonName.charAt(0).toUpperCase() + buttonName.slice(1)}`,
        className: buttonName.toLowerCase() + 'Btn'
    };

    if (buttonName === 'Add') {
        baseModalButton.action = function (e, dt, node, config) {
            showPopupModal(viewModelName, buttonName);
        };
    } else if (buttonName === 'Edit') {
        baseModalButton.extend = 'selectedSingle';
        baseModalButton.action = function (e, dt, button, config) {
            var oid = dt.row({ selected: true }).data().oid;
            showPopupModal(viewModelName, buttonName, { 'id': oid });
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



function showPopupModal(viewModel, action, additionalData = {}) {
    var modalBodyId = action.toLowerCase() + "ModalBody";
    var modalId = "kt_modal_3";
    var url = `GenericTable/AddPopup`;


    // Pass viewModel as a query parameter
    url += "?modelName=" + viewModel;


    $("#" + modalBodyId).load(url, additionalData,
        function (response, status, xhr) {
            if (status === "error") {
                handleErrors(xhr.status);
            } else {
                $('#' + modalId).modal('show');

                document.getElementById("createButton").addEventListener('click', function (event) {
                    // Create an object to store form data
                    var formDataObject = {};

                    var formData = new FormData(document.getElementById("createForm"));
                    formData.forEach(function (value, key) {
                        formDataObject[key] = value;
                    });

                    formDataObject['modelType'] = viewModel;

                    // Send the form data to the controller using AJAX
                    $.ajax({
                        url: $("#createForm").attr("action"),
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(formDataObject),
                        success: function (response) {

                            if (response.success === false) {

                                // if there is stuff in error list, problem is probably validation error
                                // otherwise, an internal error happened (e.g., num of rows affected was 0)

                                if (response.errors == null) {
                                    // show failure message
                                    Swal.fire({
                                        icon: 'error',
                                        title: 'An unexpected error happened',
                                        showConfirmButton: false,
                                        timer: 2000
                                    })
                                }
                                else {
                                    // display validation errors
                                    var errorMessages = response.errors.join('<br>');
                                    $('#errorContainer').html(errorMessages);
                                }

                            }
                            else {

                                $('#addModal').modal('hide'); // hide the add modal
                                $('#table').DataTable().ajax.reload(); // reload the grid

                                // show success message
                                Swal.fire({
                                    icon: 'success',
                                    title: 'New entry saved',
                                    showConfirmButton: false,
                                    timer: 1000
                                })

                            }

                            KTApp.hidePageLoading();
                        },
                        error: function (error) {

                          

                            // show error message
                            Swal.fire({
                                icon: 'error',
                                title: 'An unexpected error occured: ' + error
                            })

                            KTApp.hidePageLoading();
                        }
                    });
                });

            }
        });
}

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
        var url = `/${viewModel}/Delete`;

        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(oids),
                contentType: "application/json",
                success: function (response) {
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
                },
                error: function (error) {
                    handleErrors(401);
                }
            });
        }
    });
}

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