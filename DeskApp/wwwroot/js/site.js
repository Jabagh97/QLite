function connectToHub(clientType, clientName, clientID) {
    var huburl = `http://localhost:7258/communicationHub/?clientType=${clientType}&clientName=${clientName}&clientId=${clientID}`;
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


    connection.on("NotifyTicketState", function (message) {
        try {
            // Parse the JSON string into a JavaScript object
            var messageObject = JSON.parse(message);

            // Access the TicketStateValue property from the message object
            var ticketStateValue = messageObject.TicketStateValue;

            var state = '';

            // Determine which fetchTickets function to call based on ticketStateValue

            //TODO: Handle Ticket Update in a better way 

            switch (ticketStateValue) {
                case 0:
                    fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');
                    state = 'in Waiting List ';
                    break;
                case 1:
                    fetchTickets('Ticket/GetTransferedTickets', 'Transfered Tickets', 'TT');
                    state = 'Transfered';

                case 2:
                    fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');
                    updateMainPanel();


                    break;
                case 3:
                    fetchTickets('Ticket/GetParkedTickets', 'Parked Tickets', 'PT');
                    state = 'Parked';

                case 4:
                    fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');
                    fetchCompletedTickets('Ticket/GetCompletedTickets');
                    updateMainPanel();


                    break;
                default:
                    console.error("Unknown TicketStateValue:", ticketStateValue);
                    fetchTickets('Ticket/GetWaitingTickets', 'Waiting Tickets', 'WT');

                    break;
            }

            // Show a notification using SweetAlert
            Swal.fire({
                position: 'top-end',
                icon: 'info', // Set icon to "info"
                title: '<span style="font-size: 20px;">New Ticket</span>',
                html: `
        <div style="font-size: 16px; line-height: 1.5;">
            New ticket ${state}:<br>
            Number: ${messageObject.TicketNumber}<br>
            Segment: ${messageObject.SegmentName}<br>
            Service: ${messageObject.ServiceTypeName}
        </div>
    `,
                showConfirmButton: false,
                timer: 3000, // 3 seconds
                backdrop: false // Disable backdrop
            });

        } catch (error) {
            console.error("Error processing NotifyTicketState message:", error);
        }
    });

    connection.onclose(startConnection);

}
