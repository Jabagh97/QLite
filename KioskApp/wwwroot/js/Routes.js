function loadSegmentView() {
    $.ajax({
        url: 'GetSegmentView',
        type: 'GET',
        data: { HwID: kioskID },
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
        url: 'GetServiceView', 
        type: 'GET',
        data: { segmentOid: segmentOid, hwId: kioskID }, 
        success: function (response) {
            console.log("Service");
            $('#content').html(response);
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}


function loadTicketView(ticket) {
    $.ajax({
        url: 'GetTicketView',
        type: 'GET',
        data: { ticketJson: JSON.stringify(ticket) }, 

        success: function (response) {
            console.log("Ticket");
            $('#content').html(response);
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}


function PrintTicket(ticket) {
    $.ajax({
        url: 'PrintTicket',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ html: ticket }), 
        success: function (response) {
            console.log("Printed");
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function DisplayTicket(ticket) {
    $.ajax({
        url: 'DisplayTicket', 
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ DisplayNo: ticket.DisplayNo, TicketNo: ticket.ServiceCode + ticket.TicketNumber, SendToMain: true }), 
        success: function (response) {
            console.log("Ticket displayed successfully");
        },
        error: function (xhr, status, error) {
            console.error("Error displaying ticket:", xhr.responseText);
        }
    });
}




function changeLanguage(step,lang) {
    $.ajax({
        url: `ChangeLanguage?LangID=${lang}&step=${step}`,
        type: 'POST',
        success: function (response) {
            console.log("language changed successfully");

            $('#content').html(response);

        },
        error: function (xhr, status, error) {
            console.error("Error cahnging language", xhr.responseText);
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

function postHttpReq(url, data) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: 'POST', 
            data: data,
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                resolve(response); // Resolve promise with response data
            },
            error: function (xhr, status, error) {
                reject(error); // Reject promise with error
            }
        });
    });
}



