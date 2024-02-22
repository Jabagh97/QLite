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

function loadTicketView(ticket) {
    $.ajax({
        url: '/Ticket/Index',
        type: 'GET',
        data: { Ticket: ticket },

        success: function (response) {
            console.log("Ticket")
            $('#content').html(response);

        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

async function getHttpReq(url, data) {
    let temp;

    await $.ajax({
        url: url,
        type: 'Get',
        data: data,
        contentType: 'application/json; charset=utf-8',
        async: true,
        success: function (value) {
            console.log(url + " response:" + JSON.stringify(value));
            temp = value;
            return temp;
        },
        error: async function (ex) {
           
        }
    });
    return temp;
}

async function postHttpReq(url, data) {
    let temp;

    await $.ajax({
        url: url,
        type: 'Post',
        data: data,
        contentType: 'application/json; charset=utf-8',
        async: true,
     
        success: function (value) {
            temp = value;
            return temp;
        },
        error: async function (ex) {
        
        }
    });
    return temp;
}