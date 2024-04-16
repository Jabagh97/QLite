function connectToHub(clientType, clientName, apiUrl) {

    clientID = 'Admin';

    var huburl = apiUrl + `/communicationHub/?clientType=${clientType}&clientName=${clientName}&clientId=${clientID}`;
    var connection = new signalR.HubConnectionBuilder().withUrl(huburl).build();

    var retryInterval = 5000; // Retry connection every 5 seconds
    function startConnection() {
        connection.start().then(function () {

            console.log("Connected to Communication Hub.");

            // Connection successful, hide error message and show content
            $("#connectionError").hide();
            $("#content").show();
            $("#sideBar").show();

        }).catch(function (err) {
            console.error(err.toString());

            // Connection failed, show error message and retry after interval
            $("#connectionError").show();
            $("#sideBar").hide();

            $("#content").hide();

            setTimeout(startConnection, retryInterval);
        });
    }


    startConnection();

    connection.onclose(startConnection);

}

