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
        url: 'Ticket/PrintTicket',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ html: ticket }), // Convert data to JSON string
        success: function (response) {
            console.log("Printed");
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
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


var brokenDevices = [];


async function queryHw() {
    let status = await getHttpReq('Home/CheckKiosk');

    if (status.ok)
        setBrokenDevices(undefined);
    else
        setBrokenDevices(status.hwStatusList);
    evalUnavStatus("queryHw", true);
}


function setBrokenDevices(hwStatusList) {
    console.log("setBrokenDevices:" + hwStatusList)
    if (!hwStatusList || hwStatusList.length == 0) {
        console.log("no broken device")

        brokenDevices = [];
        return;
    }
    brokenDevices = hwStatusList.map(function (v) {
        return v.device;
    });
}

function evalUnavStatus(caller, noRunProc) {
    try {
        if (!brokenDevices || brokenDevices.length === 0) {
            $("#content").show();
            $("#Errors").hide();
            $("body").unbind("click");

            $("body").on('click', function (event) {
                touch();
            });
        } else {
            $("#content").hide();

            $("body").unbind("click");

            $("#Errors").show();

            // Reset all images to hide them
            $("#Printer, #Display, #Terminal").hide();

            // Show images based on the number of broken devices
            if (brokenDevices.length === 1) {
                if (brokenDevices[0] === 0) {
                    $("#Printer").show();
                } else if (brokenDevices[0] === 1) {
                    $("#Display").show();
                } else if (brokenDevices[0] === 2) {
                    $("#Terminal").show();
                }
            } else if (brokenDevices.length === 3) {
                $("#Printer, #Display,#Terminal").show();
            }
        }
    } catch (e) {
        console.log("evalUnavStatus error:" + e);
    }
}
