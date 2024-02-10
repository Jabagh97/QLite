    // Initialize DataTable
    var table = $('#table').DataTable({
        serverSide: false,
        paging: true,
        filter: true,
        columns: [
            { data: 'TicketNumber' },
            { data: 'Service' },
            { data: 'Segment' },
            {
                data: null,
                render: function (data, type, row) {
                    return '<button class="btn btn-primary call-ticket" data-oid="' + data.Oid + '">+</button>';
                }
            }
        ]
    });

    // Function to fetch waiting tickets
    function getWaitingTickets() {
        $.ajax({
            url: '/Ticket/GetWaitingTickets',
            type: 'GET',
            success: function (data) {
                // Clear existing rows
                table.clear().draw();
                // Add new rows
                $.each(data, function (index, item) {
                    table.row.add({
                        TicketNumber: item.TicketNumber,
                        Service: item.Service,
                        Segment: item.Segment,
                        Oid: item.Oid
                    }).draw();
                });
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    }

    // Function to fetch parked tickets
    function getParkedTickets() {
        $.ajax({
            url: '/Ticket/GetParkedTickets',
            type: 'GET',
            success: function (data) {
                // Clear existing rows
                table.clear().draw();
                // Add new rows
                $.each(data, function (index, item) {
                    table.row.add({
                        TicketNumber: item.TicketNumber,
                        Service: item.Service,
                        Segment: item.Segment,
                        Oid: item.Oid
                    }).draw();
                });
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    }

    // Function to fetch transferred tickets
    function getTransferedTickets() {
        $.ajax({
            url: '/Ticket/GetTransferedTickets',
            type: 'GET',
            success: function (data) {
                // Clear existing rows
                table.clear().draw();
                // Add new rows
                $.each(data, function (index, item) {
                    table.row.add({
                        TicketNumber: item.TicketNumber,
                        Service: item.Service,
                        Segment: item.Segment,
                        Oid: item.Oid
                    }).draw();
                });
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    }

    // Initial load - Get waiting tickets
    getWaitingTickets();

    // Button click handlers
    $('#Waiting').click(function () {
        getWaitingTickets();
    });

    $('#Parked').click(function () {
        getParkedTickets();
    });

    $('#Transfered').click(function () {
        getTransferedTickets();
    });

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
