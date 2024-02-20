// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var huburl = 'http://localhost:7258/communicationHub/?branchId=1111&branchName=IST&clientType=Desk&clientName=$TestKIosk&clientId=62266&eventId=Waiting;Waiting_T;Service;Park;Final'
var connection = new signalR.HubConnectionBuilder().withUrl(huburl).build();

connection.start().then(function () {
    console.log("Connected to Communication Hub.");

    sendMessageToKiosk("Desk is Conncted")
}).catch(function (err) {
    return console.error(err.toString());
});

function sendMessageToKiosk(message) {
    connection.invoke("SendMessageToKiosk", message).catch(function (err) {
        return console.error(err.toString());
    });
}

// Handle click event of the sendButton
document.getElementById("sendButton").addEventListener("click", function () {
    var message = document.getElementById("messageInput").value;
    sendMessageToKiosk(message);
});

// Define an endpoint to handle incoming messages from the server
connection.on("ReceiveMessage", function (message) {
    // Process the received message here, e.g., update UI
    console.log("Message received from server:", message);
});