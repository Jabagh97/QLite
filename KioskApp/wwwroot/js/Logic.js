// Declare global variables
let branch;
let kioskType;
let KioskType;
let hwId;
let name;
let hubUrl;
let connection;
var brokenDevices = [];
const retryInterval = 5000; // Retry connection every 5 seconds

// Initialize connection
function initConnection() {
    connection = new signalR.HubConnectionBuilder().withUrl(hubUrl).build();


    const connectionKiosk = new signalR.HubConnectionBuilder()
        .withUrl('/kioskHub') 
        .build();
    // Set up event handlers
    connection.on("ReceiveMessage", function (message) {
        console.log("Message received from server:", message);
    });

    connection.on("NotifyTicketState", function (ticket) {
        try {
            const messageObject = JSON.parse(ticket);
            DisplayTicket(messageObject);
            console.log("Ticket received from server:", messageObject);
        } catch (error) {
            console.error("Error processing NotifyTicketState message:", error);
        }
    });

    connectionKiosk.on("HwEvent", function (message) {

        console.log("HwStatusChanged" + message)
        queryHw()

    });

    connectionKiosk.start().catch(err => console.error(err));

    // Handle connection close
    connection.onclose(startConnection);
}

// Start connection
function startConnection() {
    connection.start()
        .then(function () {
            console.log("Connected to Communication Hub.");
            sendMessageToDesk("Kiosk is Connected");
            hideErrorAndShowContent();
        })
        .catch(function (err) {
            console.error(err.toString());
            showErrorAndRetry();
        });
}

// Hide error message and show content
function hideErrorAndShowContent() {
    $("#connectionError").hide();
    $("#AuthError").hide();

    $("#loadingAnimation").hide();
    $("#content").show();
}

// Show error message and retry after interval
function showErrorAndRetry() {
    $("#connectionError").show();
    $("#loadingAnimation").hide();
    $("#content").hide();
    setTimeout(startConnection, retryInterval);
}

// Handle touch event
function touch() {
    $("body").unbind("click");
    loadSegmentView();
}

// Initialize on window load
$(window).on('load', function () {
    $("body").unbind("click");
    $("body").on('click', function (event) {
        touch();
    });

    ValidateKiosk();


});

// Send message to desk
function sendMessageToDesk(message) {
    if (connection && connection.state === "Connected") {
        connection.invoke("SendMessageToDesk", message)
            .catch(function (err) {
                console.error(err.toString());
            });
    } else {
        console.error("Connection is not in the 'Connected' state.");
    }
}

// Validate kiosk
function ValidateKiosk() {
    $.ajax({
        url: '/KioskAuth/Authenticate',
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            console.log("Kiosk Authenticated successfully");
            try {
                const responseObject = JSON.parse(response);
                if (responseObject && responseObject.length > 0) {
                    const kioskData = responseObject[0];
                    ({ branchID, kioskType, hwId, name } = kioskData);
                    KioskType = (kioskType === 0) ? "Kiosk" : (kioskType === 1) ? "Display" : null;
                    if (KioskType) {
                        hubUrl = `${apiUrl}/communicationHub/?branchId=${branch}&branchName=IST&clientType=${KioskType}&clientName=${name}&clientId=${hwId}`;
                        initConnection();
                        startConnection();
                        queryHw();
                    } else {
                        console.error("Invalid kioskType:", kioskType);
                    }
                } else {
                    console.error("No kiosk data found in response.");
                }
            } catch (error) {
                console.error("Error parsing JSON response:", error);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error :", xhr.responseText);
            $("#AuthError").show();
            $("#loadingAnimation").hide();
            $("#content").hide();
            setTimeout(ValidateKiosk, retryInterval);

        }
    });
}

// Query hardware status
async function queryHw() {
    const status = await getHttpReq('Home/CheckKiosk');
    if (status.ok) {
        setBrokenDevices(undefined);
    } else {
        setBrokenDevices(status.hwStatusList);
    }
    evalUnavStatus("queryHw", true);
}

// Set broken devices
function setBrokenDevices(hwStatusList) {
    if (!hwStatusList || hwStatusList.length === 0) {
        brokenDevices = [];
    } else {
        brokenDevices = hwStatusList.map(v => v.device);
    }
}

// Evaluate unavailable status
function evalUnavStatus(caller, noRunProc) {
    try {
        if (!brokenDevices || brokenDevices.length === 0) {
            showContent();
        } else {
            hideContentAndShowErrors();
            displayBrokenDevices();
        }
    } catch (e) {
        console.log("evalUnavStatus error:", e);
    }
}

// Show content and bind touch event
function showContent() {
    $("#content").show();
    $("#Errors").hide();
    $("body").off("click").on('click', touch);
}

// Hide content and show errors
function hideContentAndShowErrors() {
    $("#content").hide();
    $("body").off("click");
    $("#Errors").show();
}

// Display broken devices
function displayBrokenDevices() {
    $("#Printer, #Display, #Terminal").hide();
    if (brokenDevices.length === 1) {
        if (brokenDevices[0] === 0) {
            $("#Printer").show();
        } else if (brokenDevices[0] === 1) {
            $("#Display").show();
        } else if (brokenDevices[0] === 2) {
            $("#Terminal").show();
        }
    } else if (brokenDevices.length === 3) {
        $("#Printer, #Display, #Terminal").show();
    }
}
