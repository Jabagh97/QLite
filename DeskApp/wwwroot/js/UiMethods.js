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
    updateMainPanel();
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
    var note = $('#ticketNote').val();
    var ticket = $('#ticketNumber').data('ticket-id');
    $.ajax({
        url: 'Ticket/ParkTicket',
        type: 'POST',
        data: { TicketId: ticket, DeskID: deskId, TicketNote: note },
        success: function (response) {
            $('#parkTicketModal').modal('hide');

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
    var note = $('#ticketNote').val();
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


function GetCreatableServices(DeskID) {
    $.ajax({
        url: 'Ticket/GetCreatableServicesList?DeskID=' + DeskID,
        type: 'GET',
        success: function (response) {
            // Clear any existing dropdown options
            $('#serviceDropdown').empty();

            // Populate dropdown with desks from the response
            response.forEach(function (desk) {
                $('#serviceDropdown').append('<option value="' + desk.oid + '">' + desk.name + '</option>');
            });

            
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


        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
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
    });
}

fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT', true);

fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT', true);


fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');

// Call the function to fetch completed tickets on page load
fetchCompletedTickets('Ticket/GetCompletedTickets?deskId=' + deskId);

GetDesk();

fetchAndPopulateMacros();
