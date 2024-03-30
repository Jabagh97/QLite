html2canvas(document.querySelector("#ticketdiv")).then(canvas => {

    var imgData = canvas.toDataURL();

    // Create a link and set the URL as the href
    var link = document.createElement('a');
    link.href = imgData;
    link.download = 'TicketImage.png'; // Set the download name

    // Append the link to the body, click it, and remove it
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);


});