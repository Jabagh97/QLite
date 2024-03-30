// Wait for the DOM to be fully loaded
$(document).ready(function () {
    var body = document.getElementById("ticket-body");

    //Apply the transition style to the ticket-body
    body.style.transition = "transform 2s ease-in-out";

    // Animate the ticket-body sliding up
    body.style.transform = "translateY(+" + 1500 + "px)";

    // After 2 seconds, redirect to the index page
    setTimeout(function () {
        window.location.href = '/';
    }, 2000);

    var html = $("#ticketdiv").html();

    PrintTicket(html);



});

