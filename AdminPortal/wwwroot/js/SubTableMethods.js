function loadTabData(tabName, modelName, Oid) {
    // Destroy existing DataTable before loading new data
    if ($.fn.dataTable.isDataTable('#dynamicTabTable')) {
        $('#dynamicTabTable').DataTable().destroy();
    }

    $.ajax({
        type: "POST",
        url: "/GenericTable/LoadTabData",
        data: { tabName: tabName, modelName: modelName, Oid: Oid },
        dataType: "json", // specify dataType as 'json' to automatically parse the JSON response
        success: function (response) {
            // Handle the response from the server if needed
            if (response.status === "success") {
                var innerData = JSON.parse(response.data);

                if (innerData.data.length < 1) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'No Data Found '
                    });
                    return;
                }


                // Update the table header
                var tableHead = $('#dynamicTabTable thead');
                tableHead.empty();
                var headerRow = $('<tr>');
                for (var prop in innerData.data[0]) {
                    // Skip "oid" property
                    if (prop !== "oid") {
                        headerRow.append('<th>' + prop + '</th>');
                    }
                }
                tableHead.append(headerRow);

                // Update the table body
                var tableBody = $('#dynamicTabTable tbody');
                tableBody.empty();
                for (var i = 0; i < innerData.data.length; i++) {
                    var row = $('<tr data-oid="' + innerData.data[i].oid + '">'); // Add data-oid attribute
                    for (var prop in innerData.data[i]) {
                        // Skip "oid" property
                        if (prop !== "oid") {
                            row.append('<td>' + innerData.data[i][prop] + '</td>');
                        }
                    }
                    tableBody.append(row);
                }

                // Initialize DataTables with pagination
                $('#dynamicTabTable').DataTable({
                    select: true,
                    dom: 'Bfrtilp',
                    columnDefs: [
                        { targets: '_all', "defaultContent": "" },
                        {
                            targets: '_all',
                            "render": function (data, type, row, meta) {
                                if (String(data).length <= 20) {
                                    return data
                                } else {
                                    return data.substring(0, 20) + ".....";
                                }
                            }
                        }
                    ],
                    lengthMenu: [5, 10, 25, 50, 100],
                    pageLength: 5,
                    buttons: generateTabButtons(tabName, modelName)
                });
            } else {
                // Handle the case where the server response is not success
                Swal.fire({
                    icon: 'error',
                    title: 'An unexpected error occurred'
                });
            }
        },
        error: function (xhr, status, error) {
            // Handle errors
            Swal.fire({
                icon: 'error',
                title: 'An error occurred while processing the request'
            });
        }
    });
}

function deleteSelectedRows(tabName, modelName) {
    var selectedRows = $('#dynamicTabTable').DataTable().rows({ selected: true }).nodes();

    if (selectedRows.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'No rows selected for deletion'
        });
        return;
    }

    // Extract oid values from the selected rows
    var selectedOids = $(selectedRows).map(function () {
        return $(this).data('oid');
    }).get();
    var formData = new FormData(document.getElementById("editForm"));
    var modelOid = formData.get("Oid");



    // Send the list of selected Oids to the controller
    $.ajax({
        type: 'POST',
        contentType: 'application/json',

        url: '/GenericTable/DeleteSelectedRows',
        data: JSON.stringify({
            selectedOids: selectedOids,
            tabName: tabName,
            modelName: modelName,
            modelOid: modelOid
        }),
        success: function (response) {
            if (response.status === "success") {
                // Load new data into the table
                var table = $('#dynamicTabTable').DataTable();
                table.clear().draw();
                loadTabData(tabName, modelName, modelOid);

                // Show success confirmation using Swal.fire
                Swal.fire({
                    icon: 'success',
                    title: 'Rows deleted successfully',
                    timer: 1500, // Adjust the timer as needed
                    showConfirmButton: false
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'An unexpected error occurred during deletion: ' + response.error
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                icon: 'error',
                title: 'An unexpected error occurred during deletion: ' + error
            });
        }
    });
}

function AddToSelectedRow(tabName, modelName) {


}
function generateTabButtons(tabName, modelName) {
    var buttons = [
        // Include  buttons here
        'copy',
    ];

    if (modelName !== 'KioskPkg' && !modelName.includes('KioskRequest')) {
        // Add the Delete button with a custom action
        buttons.unshift(
            //{
            //    text: 'Add',
            //    action: function (e, dt, node, config) {
            //        AddToSelectedRow(tabName, modelName);
            //    }
            //},
            {
                text: 'Delete',
                action: function (e, dt, node, config) {
                    deleteSelectedRows(tabName, modelName);
                }
            }
        );

    }

    return buttons;
}
