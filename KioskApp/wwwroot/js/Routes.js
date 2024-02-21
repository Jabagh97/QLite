function loadSegmentView() {
    $.ajax({
        url: '/Segment/Index',
        type: 'GET',
        success: function (response) {
            console.log("Segments")
            $('#content').html(response);

        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

function loadServiceView(segmentOid) {


    $.ajax({
        url: '/Service/Index',
        type: 'GET',
        data: { SegmentOid: segmentOid },

        success: function (response) {
            console.log("Service")
            $('#content').html(response);
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

function loadTicketView(serviceOid) {
    $.ajax({
        url: '/Ticket/Index',
        type: 'GET',
        data: { ServiceOid: serviceOid },

        success: function (response) {
            console.log("Ticket")
            $('#content').html(response);

        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}