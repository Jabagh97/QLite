function connectToHub(clientType, clientName, clientID, apiUrl) {

    clientID = $('#deskName').data('desk-id');

    var huburl = apiUrl + `/communicationHub/?clientType=${clientType}&clientName=${clientName}&clientId=${clientID}`;
    var connection = new signalR.HubConnectionBuilder().withUrl(huburl).build();

    var retryInterval = 5000; // Retry connection every 5 seconds
    function startConnection() {
        connection.start().then(function () {

            console.log("Connected to Communication Hub.");
            sendMessageToKiosk("Desk is Connected");

            // Connection successful, hide error message and show content
            $("#connectionError").hide();
            $("#loadingAnimation").hide();
            $("#content").show();

        }).catch(function (err) {
            console.error(err.toString());

            // Connection failed, show error message and retry after interval
            $("#connectionError").show();
            $("#loadingAnimation").hide();
            $("#content").hide();

            setTimeout(startConnection, retryInterval);
        });
    }


    function sendMessageToKiosk(message) {
        connection.invoke("SendMessageToKiosk", message).catch(function (err) {
            console.error(err.toString());
        });
    }

    startConnection();


    // Define an endpoint to handle incoming messages from the server
    connection.on("ReceiveMessage", function (message) {
        // Process the received message here, e.g., update UI
        console.log("Message received from server:", message);
    });

    //TODO fix: Popups are Closing when invoking this method
    connection.on("NotifyTicketState", function (message) {
        try {
            // Parse the JSON string into a JavaScript object
            var messageObject = JSON.parse(message);

            // Access the TicketStateValue property from the message object
            var ticketStateValue = messageObject.TicketStateValue;

            var state = '';

            var tableTitleText = document.getElementById('TableTitle')?.textContent ?? '';


            switch (ticketStateValue) {
                case 0://waiting
                    if (tableTitleText !== 'Waiting Tickets') {
                        fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT', true);

                    }
                    else {
                        fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');
                    }
                    state = 'in Waiting List ';

                    showPopup(state, messageObject.TicketNumber, messageObject.SegmentName, messageObject.ServiceTypeName)
                    break;
                case 1://transfer

                    if (tableTitleText !== 'Transferred Tickets') {

                        fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT', true);

                    }
                    else {

                        fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT');
                    }
                    state = 'Transfered';

                    showPopup(state, messageObject.TicketNumber, messageObject.SegmentName, messageObject.ServiceTypeName)
                    break;

                case 2://service
                    if (tableTitleText === 'Waiting Tickets') {
                        fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');
                        fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT', true);
                        fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT', true);
                    }
                    else if (tableTitleText === 'Transferred Tickets') {
                        fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT');
                        fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT', true);
                        fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT', true);
                    }
                    else if (tableTitleText === 'Parked Tickets') {
                        fetchTickets('Ticket/GetTransferedTickets?deskId=' + deskId, 'Transfered Tickets', 'TT',true);
                        fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT', true);
                        fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT');

                    }

                    break;
                case 3://park

                    if (tableTitleText !== 'Transferred Tickets') {
                        fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT', true);

                    }
                    else {

                        fetchTickets('Ticket/GetParkedTickets?deskId=' + deskId, 'Parked Tickets', 'PT');

                    }

                    break;
                case 4://done
                    //fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');
                    fetchCompletedTickets('Ticket/GetCompletedTickets?deskId=' + deskId);
                    updateMainPanel();


                    break;
                default:
                    console.error("Unknown TicketStateValue:", ticketStateValue);
                    fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');

                    break;
            }

            // Show a notification using SweetAlert

        } catch (error) {
            console.error("Error processing NotifyTicketState message:", error);
            fetchCompletedTickets('Ticket/GetCompletedTickets?deskId=' + deskId);
            updateMainPanel();
            Swal.fire({
                position: 'center',
                icon: 'error',
                title: '<span style="font-size: 20px;color :aliceblue;">No eligible tickets found to call from this Macro.</span>',
                showConfirmButton: false,
                timer: 3000, // 3 seconds
                backdrop: true // Disable backdrop
            });

        }
    });

    connection.onclose(startConnection);

}


function showPopup(state, TicketNumber, SegmentName, ServiceTypeName) {

    Swal.fire({
        position: 'top-end',
        icon: 'info',
        title: '<span style="font-size: 20px;">New Ticket state</span>',
        html: `
        <div style="font-size: 16px; line-height: 1.5;">
            New ticket ${state}:<br>
            Number: ${TicketNumber}<br>
            Segment: ${SegmentName}<br>
            Service: ${ServiceTypeName}
        </div>
    `,
        showConfirmButton: false,
        timer: 3000, // 3 seconds
        backdrop: false // Disable backdrop
    });

}

// Define initial positions for draggables
const positions = {
    draggable1: { x: 0, y: 0 },
    draggable2: { x: 0, y: 0 },
    draggable3: { x: 0, y: 0 }
};

// Function to initialize draggables
function initializeDraggable(selector, position) {
    interact(selector).draggable({
        listeners: {
            start(event) {
                event.target.style.zIndex = '1000'; // Bring dragged element to the front
            },
            move(event) {
                position.x += event.dx;
                position.y += event.dy;

                event.target.style.transform = `translate(${position.x}px, ${position.y}px)`;
            }
        },
        modifiers: [
            interact.modifiers.restrictRect({
                restriction: document.querySelector('#BodyContainer'),
                endOnly: true,
            })
        ],
        inertia: true
    });
}

document.addEventListener('mouseover', function (event) {
    const element = event.target;
    if (element.classList.contains('drag-icon')) {
        initializeDraggable('.draggable', positions.draggable1);
        initializeDraggable('.draggable2', positions.draggable2);
        initializeDraggable('.draggable3', positions.draggable3);
    } else {
        interact('.draggable').draggable({
            enabled: false  // explicitly disable dragging
        });
        interact('.draggable2').draggable({
            enabled: false  // explicitly disable dragging
        });
        interact('.draggable3').draggable({
            enabled: false  // explicitly disable dragging
        });
    }
});
