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
}


fetchTickets('Ticket/GetParkedTickets', 'Parked Tickets', 'PT',true);

fetchTickets('Ticket/GetTransferedTickets', 'Transfered Tickets', 'TT',true);

fetchTickets('Ticket/GetWaitingTickets','Waiting Tickets','WT');



// Call ticket button click handler
$('#table').on('click', '.call-ticket', function () {
    var oid = $(this).data('oid');
    $.ajax({
        url: '/Ticket/CallTicket',
        type: 'POST',
        data: { oid: oid },
        success: function (data) {
            // Handle success if needed
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
});
