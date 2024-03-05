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
    $('#TableTitle').text(title);

    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {

            var x = response.data.length;

            // Update the waiting number
            $('#' + id).text(response.data.length);
            if (justnumber) { return }
            // Clear existing rows
            table.clear().draw();

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

    fetchCompletedTickets('Ticket/GetCompletedTickets');
    updateMainPanel();
}


fetchTickets('Ticket/GetParkedTickets', 'Parked Tickets', 'PT',true);

fetchTickets('Ticket/GetTransferedTickets', 'Transfered Tickets', 'TT',true);

fetchTickets('Ticket/GetWaitingTickets','Waiting Tickets','WT');





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

// Call the function to fetch completed tickets on page load
fetchCompletedTickets('Ticket/GetCompletedTickets');



function updateMainPanel()
{
    var deskId = $('#deskName').data('desk-id');

    $.ajax({
        url: 'Ticket/GetCurrentTicket',
        type: 'GET',
        data: { DeskID: deskId},

        success: function (response) {

            $('#mainPanelContent').html(response);



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
    var deskId = $('#deskName').data('desk-id');
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

function autocall()
{
    $.ajax({
        url: '/Ticket/CallTicket',
        type: 'POST',
      
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
    var deskId = $('#deskName').data('desk-id');

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
    var deskId = $('#deskName').data('desk-id');

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
    var deskId = $('#deskName').data('desk-id');
    $.ajax({
        url: '/Ticket/GetDesk?deskId=' + deskId,
        type: 'GET',
        success: function (data) {
            $('#deskName').text(data); // Update the content with the fetched desk name
        },
        error: function () {
            $('#deskName').text('Error fetching desk name'); // Display error message if request fails
        }
    });
}
GetDesk();