var deskId = $('#deskName').data('desk-id');


// Initialize DataTable
var table = $('#table').DataTable({
    serverSide: false,
    paging: true,
    filter: true,
    columns: [
        { data: 'ticketNumber' },
        { data: 'service' },
        { data: 'segment' },
        {
            data: 'oid',
            render: function (data) {
                return `<button class="btn btn-primary call-ticket" data-oid="${data}">+</button>`;
            }
        }
    ],
    lengthMenu: [
        [5, 10, 25, 50, 1000],
        ['5 rows', '10 rows', '25 rows', '50 rows', 'Show all']
    ],
    pageLength: 5
});

// Function to fetch tickets by type
function fetchTickets(url, title,id,justnumber) {
    // Set the title text

    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {

            var x = response.data.length;
            // Update the waiting number
            $('#' + id).text(response.data.length);

            //return if only the number of tickets is requested
            if (justnumber) { return }

            // Clear existing rows
            table.clear().draw();

            $('#TableTitle').text(title);

            // Add new rows from the received data
            $.each(response.data, function (index, item) {
                table.row.add({
                    ticketNumber: item.ticketNumber,
                    service: item.service,
                    segment: item.segment,
                    oid: item.oid
                }).draw();
            });

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });

    fetchCompletedTickets('Ticket/GetCompletedTickets?deskId=' + deskId);
}

var CompletedTable = $('#CompletedTable').DataTable({
    paging: true,
    lengthChange: true,
    searching: true,
    ordering: true,
    info: true,
    autoWidth: false,
    pageLength: 5,
    columns: [
        { data: 'ticketNumber' },
        { data: 'service' },
        { data: 'segment' },
        {
            data: 'oid',
            render: function (data) {
                return `<button class="btn btn-primary call-ticket" data-oid="${data}">+</button>`;
            }
        }
    ],
    lengthMenu: [
        [5, 10, 25, 50, -1],
        ['5 rows', '10 rows', '25 rows', '50 rows', 'Show all']
    ]
});

// Function to fetch completed tickets
function fetchCompletedTickets(url) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            // Clear existing rows
            CompletedTable.clear().draw();

            // Add new rows from the received data
            $.each(response.data, function (index, item) {
                CompletedTable.row.add({
                    ticketNumber: item.ticketNumber,
                    service: item.service,
                    segment: item.segment,
                    oid: item.oid
                }).draw();
            });
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function updateMainPanel()
{

    $.ajax({
        url: 'Ticket/GetCurrentTicket',
        type: 'GET',
        data: { DeskID: deskId},

        success: function (response) {

            $('#mainPanelContent').html(response);

            //var selectedMacroName = localStorage.getItem('selectedMacroName');

            //// If there is a selectedMacroName, set the text of the element with id macroName
            //if (selectedMacroName != null && selectedMacroName.trim() !== '') {
            //    $('#macroName').text(selectedMacroName);

            //    // Remove the 'hidden' attribute to show the button
            //    $("#autoCallBtn").removeAttr('hidden');
            //} else {
            //    // If selectedMacroName is null or empty, hide the button
            //    $("#autoCallBtn").attr('hidden', 'hidden');
            //}

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });

}

function EndTicket()
{

    $.ajax({
        url: 'Ticket/EndTicket',
        type: 'GET',
        data: { DeskID: deskId },
        success: function (response) {
            updateMainPanel();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function showParkTicketModal() {
    $('#parkTicketModal').modal('show');
}
function ParkTicket() {
    var note = $('#parkTicketNote').val();
    var ticket = $('#ticketNumber').data('ticket-id');
    $.ajax({
        url: 'Ticket/ParkTicket',
        type: 'POST',
        data: { TicketId: ticket, DeskID: deskId, TicketNote: note },
        success: function (response) {
            $('#parkTicketModal').modal('hide');
            updateMainPanel();

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            $('#parkTicketModal').modal('hide');

        }
    });
}
function showTransferTicketModal() {
    $('#transferTicketModal').modal('show');
}

function TransferTicket() {
    var note = $('#transferTicketNote').val();
    var ticket = $('#ticketNumber').data('ticket-id');
    var transferServiceType = $('#serviceDropdown').val(); // Get selected value from service dropdown
    var transferToDesk = $('#deskDropdown').val(); // Get selected value from desk dropdown

    $.ajax({
        url: 'Ticket/TransferTicket',
        type: 'POST',
        data:
        {
            TicketId: ticket,
            TransferServiceType: transferServiceType,
            TransferToDesk: transferToDesk,
            TransferFromDesk: deskId,
            TicketNote: note
        },
        success: function (response) {
            $('#transferTicketModal').modal('hide');
            updateMainPanel();

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            $('#transferTicketModal').modal('hide');

        }
    });
}
function GetDeskListAndServices() {
    GetDeskList();
    GetTransferableServices(deskId);
}

function GetDeskList() {
    $.ajax({
        url: 'Ticket/GetDeskList',
        type: 'GET',
        success: function (response) {
            // Clear any existing dropdown options
            $('#deskDropdown').empty();

            // Populate dropdown with desks from the response
            response.forEach(function (desk) {
                $('#deskDropdown').append('<option value="' + desk.oid + '">' + desk.name + '</option>');
            });
            // Show the modal once the dropdown is populated
            $('#transferTicketModal').modal('show');
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            // Handle error
        }
    });
}

function GetTransferableServices(DeskID) {
    $.ajax({
        url: 'Ticket/GetTransferableServiceList?DeskID=' + DeskID,
        type: 'GET',
        success: function (response) {
            // Append dropdown options with desks from the response
            response.forEach(function (service) {
                $('#serviceDropdown').append('<option value="' + service.serviceType + '">' + service.serviceTypeNavigation.name + '</option>');
            });

            // No need to show the modal again here, as it's already shown by GetDeskList
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            // Handle error
        }
    });
}


function autocall() {
    var macroId = localStorage.getItem('selectedMacroId'); 

    $.ajax({
        url: '/Ticket/CallTicket',
        type: 'POST',
        data: { DeskID: deskId, MacroID: macroId },

        success: function (data) {
            // Handle success if needed
            updateMainPanel();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
// Call ticket button click handler
$('#table').on('click', '.call-ticket', function () {
    var oid = $(this).data('oid');

    $.ajax({
        url: '/Ticket/CallTicket',
        type: 'POST',
        data: { TicketID: oid, DeskID: deskId },
        success: function (data) {
            // Handle success if needed
            updateMainPanel();

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
});
// Call ticket button click handler
$('#CompletedTable').on('click', '.call-ticket', function () {
    var oid = $(this).data('oid');

    $.ajax({
        url: '/Ticket/CallTicket',
        type: 'POST',
        data: { TicketID: oid, DeskID: deskId },
        success: function (data) {
            // Handle success if needed
            updateMainPanel();

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
});

// Handle double click on rows
$('#CompletedTable tbody').on('dblclick', 'tr', function () {
    var rowData = CompletedTable.row(this).data(); // Get the data of the clicked row
    var ticketID = rowData.oid; // Get the oid of the clicked row
    ShowTicketStateModal(ticketID); // Call the function with ticketID
});
function GetDesk()
{
    $.ajax({
        url: '/Ticket/GetDesk?deskId=' + deskId,
        type: 'GET',
        success: function (data) {
            $('#deskName').text(data.name); // Update the content with the fetched desk name
        },
        error: function () {
            $('#deskName').text('Error fetching desk name'); // Display error message if request fails
        }
    });
}


function fetchAndPopulateMacros() {
    // Send a GET request to fetch macros with the deskId
    $.get('Ticket/GetMacros', { DeskID: deskId }, function (macros) {
        // Get the macroMenu element
        var macroMenu = document.getElementById('macroMenu');

        // Clear existing menu items
        macroMenu.innerHTML = '';

        // Check if macros array is empty
        if (macros.length === 0) {
            var noMacroMessage = document.createElement('div');
            noMacroMessage.innerText = 'No Macros assigned for this desk.';
            macroMenu.appendChild(noMacroMessage);
        } else {
            // Iterate over the macros array and add dropdown items
            macros.forEach(function (macro) {
                var menuItem = document.createElement('div');
                menuItem.className = 'menu-item';
                menuItem.innerHTML = '<a class="menu-link" >' + macro.macroName + '</a>';

                menuItem.addEventListener('click', function () {
                    // Set data-macro-id attribute to macro.oid for macroMenu element
                    var macroElement = document.getElementById('macroName');
                    $('#macroName').text(macro.macroName);
                    macroElement.setAttribute('data-macro-id', macro.macro);

                    localStorage.setItem('selectedMacroId', macro.macro);
                    localStorage.setItem('selectedMacroName', macro.macroName);

                    // Remove the 'hidden' attribute to show the button
                    $("#autoCallBtn").removeAttr('hidden');
                });

                macroMenu.appendChild(menuItem);
            });
        }
    });
}


function GetSegmentsandServices()
{
    GetCreatableServices();
    GetSegmentList();
}



function GetCreatableServices() {

    $.ajax({
        url: 'Ticket/GetCreatableServicesList?DeskID=' + deskId,
        type: 'GET',
        success: function (response) {
            // Clear any existing dropdown options
            $('#createServiceDropdown').empty();

            // Populate dropdown with service from the response
            response.forEach(function (service) {
                $('#createServiceDropdown').append('<option value="' + service.serviceType + '">' + service.serviceTypeNavigation.name + '</option>');
            });
            // Show the modal once the dropdown is populated
            $('#createTicketModal').modal('show');
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            // Handle error
        }
    });

}
function GetSegmentList() {
    $.ajax({
        url: 'Ticket/GetSegmentList',
        type: 'GET',
        success: function (response) {
            // Clear any existing dropdown options
            $('#segmentDropdown').empty();

            // Populate dropdown with segment from the response
            response.forEach(function (segment) {
                $('#segmentDropdown').append('<option value="' + segment.oid + '">' + segment.name + '</option>');
            });
          
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            // Handle error
        }
    });
}
function CreateTicket() {
    var createServiceType = $('#createServiceDropdown').val(); 
    var createSegment = $('#segmentDropdown').val(); 

    $.ajax({
        url: 'Ticket/CreateTicket',
        type: 'POST',
        data:
        {
            ServiceTypeId: createServiceType,
            SegmentId: createSegment,
           
        },
        success: function (response) {
            $('#createTicketModal').modal('hide');

            Swal.fire({
                icon: 'success',
                title: 'Ticket Created Successfully',
                showConfirmButton: false,
                timer: 1500 // Auto close after 1.5 seconds
            });
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            $('#transferTicketModal').modal('hide');

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to create ticket. try another Segment.',
                confirmButtonText: 'OK'
            });

        }
    });
}

function SetBusyStatus() {
    var statusElement = document.getElementById('statusEnum');
    var status = parseInt(statusElement.getAttribute('data-status'));
    var text;

    if (status === 1) {
        // Change status to Busy (3)
        status = 3;
        text = 'Busy';
    } else {
        // Change status to Open (1)
        status = 1;
        text = 'Open';
    }

    $.ajax({
        url: 'Ticket/SetBusyStatus?DeskID=' + deskId + '&Status=' + status,
        type: 'GET',
        success: function (response) {
            // Update text content
            $('#statusEnum').text(text);
            // Update data-status attribute
            statusElement.setAttribute('data-status', status);

            // Update styling based on status
            if (status === 1) {

                // Remove sleep animation element if it exists

                $('#sleepAnimation').remove();
                // Remove dim class from body
                document.body.classList.remove('dim-screen');

                $('#busyButton').css('z-index', 'auto');

                if (!$('#kt_drawer_chat_toggle').length) {
                    $('#statusEnum').append('<div class="btn btn-icon btn-custom btn-active-light btn-active-color-primary w-35px h-35px position-relative" id="kt_drawer_chat_toggle"><span class="bullet bullet-dot bg-success h-6px w-6px  animation-blink"></span></div>');
                }
            } else {
                // Add sleep animation element
                if (!$('#sleepAnimation').length) {
                    $('#statusEnum')
                        .append(
                            '<div id="sleepAnimation" class="sleeping"><span>z</span><span>z</span><span>z</span><span>z</span></div><div class="avatar"> <div class="head"> <div class="eyes close-eyes"> <div class="eye left-eye"></div><div class="eye right-eye"></div></div> <div class="mouth yawn"></div> </div></div>');
                }
                // Add dim class to body
                document.body.classList.add('dim-screen');
                // Remove the chat toggle button if it exists
                $('#kt_drawer_chat_toggle').remove();

                $('#busyButton').css('position', 'relative');

                $('#busyButton').css('z-index', '10000');

            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            // Handle error
        }
    });
}


function ShowTicketStateModal(ticketID)
{
    // Destroy the existing DataTable instance if it exists
    if ($.fn.DataTable.isDataTable('#TicketStateTable')) {
        $('#TicketStateTable').DataTable().destroy();
    }

    var TicketStateTable = $('#TicketStateTable').DataTable({
        serverSide: false,
        paging: true,
        filter: true,
        columns: [
            { data: 'desk' },
            { data: 'callType' },
            { data: 'startTime' },
            { data: 'endTime' },
            { data: 'note' },
        ],
        ajax: {
            url: '/Ticket/GetTicketStates?TicketID=' + ticketID,
            type: "GET",
            dataType: "json",
            error: function (xhr, error, code) {
                console.log(xhr, code);
            }
        },
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
        pageLength: 5
    });

    $('#TicketStateModal').modal('show');

}

function initScreen() {
    fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT', true);

    fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT', true);


    fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');

    // Call the function to fetch completed tickets on page load
    fetchCompletedTickets('Ticket/GetCompletedTickets?deskId=' + deskId);

    GetDesk();

    fetchAndPopulateMacros();

    var selectedMacroName = localStorage.getItem('selectedMacroName');

    // If there is a selectedMacroName, set the text of the element with id macroName
    if (selectedMacroName != null && selectedMacroName.trim() !== '') {
        $('#macroName').text(selectedMacroName);

        // Remove the 'hidden' attribute to show the button
        $("#autoCallBtn").removeAttr('hidden');
    } else {
        // If selectedMacroName is null or empty, hide the button
        $("#autoCallBtn").attr('hidden', 'hidden');
    }

}

$(document).ready(function () {
    initScreen();
});

