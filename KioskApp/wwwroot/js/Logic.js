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


// Initialize when all elements on the page have loaded
$(document).ready(function () {

    ValidateKiosk();
});

// Validate kiosk
function ValidateKiosk() {
    $.ajax({
        url: '/KioskAuth/AuthenticateForWebSocket',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (kioskData) {
            console.log("Kiosk Authenticated successfully");

            if (kioskData && kioskData.kioskType !== undefined) {
                const { branch, kioskType, hwId, name } = kioskData;
                const KioskType = kioskType === 0 ? "Kiosk" : kioskType === 1 ? "Display" : null;

                if (KioskType) {
                    hubUrl = `${apiUrl}/communicationHub/?branchId=${branch}&branchName=IST&clientType=${KioskType}&clientName=${name}&clientId=${hwId}`;
                    initConnection();
                    startConnection();
                    queryHw();

                    if (KioskType === "Kiosk") {

                        $("body").off("click").on('click', function (event) {

                            // Check if the clicked element or any of its parents have a specific class or id (For Language Buttons)
                            if (!$(event.target).closest('.resize-drag').length) {
                                // If the click was not inside the language button, call touch()
                                touch();
                            }
                        });
                    }
                    else {

                        $("body").off("click");

                    }

                } else {
                    console.error(`Invalid kioskType: ${kioskType}`);
                    updateUIOnError();
                }
            } else {
                console.error("No kiosk data found in response.");
                updateUIOnError();
            }
        },
        error: function (xhr, status, error) {
            console.error(`Error: ${xhr.responseText}`);
            updateUIOnError();
            setTimeout(ValidateKiosk, retryInterval); // Make sure retryInterval is defined somewhere
        }
    });
}
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
            queryHw();

        })
        .catch(function (err) {
            console.error(err.toString());
            showErrorAndRetry();
        });
}

// Query hardware status
async function queryHw() {
    const status = await getHttpReq(`CheckKiosk`);
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

// Show content and bind touch event
function showContent() {
    $("#content").show();
    $("#Errors").hide();
    $("body").off("click").on('click', function (event) {

        // Check if the clicked element or any of its parents have a specific class or id
        if (!$(event.target).closest('.resize-drag').length) {
            // If the click was not inside the language button, call touch()
            touch();
        }
    });
}

// Hide content and show errors
function hideContentAndShowErrors() {
    $("#content").hide();
    $("body").off("click");
    $("#Errors").show();
}

// Display broken devices
function displayBrokenDevices() {
    // Hide all devices initially
    $("#Printer, #Display, #Terminal").hide();

    // If no devices are broken, nothing to display
    if (brokenDevices.length === 0) {
        return;
    }

    // If all devices are broken, show all of them
    if (brokenDevices.length === 3) {
        $("#Printer, #Display, #Terminal").show();
        return;
    }

    // Otherwise, show the broken devices
    for (let i = 0; i < brokenDevices.length; i++) {
        const device = brokenDevices[i];
        if (device === 0) {
            $("#Printer").show();
        } else if (device === 1) {
            $("#Display").show();
        } else if (device === 2) {
            $("#Terminal").show();
        }
    }
}
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

// Handle touch event
function touch() {
    $("body").off("click");
    loadSegmentView();
}


function updateUIOnError() {
    $("#AuthError").show();
    $("#loadingAnimation").hide();
    $("#content").hide();
}