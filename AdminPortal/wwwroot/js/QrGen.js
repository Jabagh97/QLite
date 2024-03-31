$(document).ready(function () {
    $('#qrType').change(function () {
        if ($(this).val() === 'app') {
            $('#appInput').show();
            $('#wifiInput').hide();
        } else {
            $('#appInput').hide();
            $('#wifiInput').show();
        }
        // Clear previous QR code
        $('#qrcode').empty();
    });


});

function generateQRCode() {
    $('#qrcode').empty(); // Clear previous QR code

    var qrType = $('#qrType').val();
    var textToEncode = "";

    if (qrType === 'app') {
        var url = $('#appUrl').val().trim();
        if (!url) {


            Swal.fire({
                title: '',
                text: 'Please enter a URL',
                icon: 'warning',
            })
            return;
        }
        textToEncode = url;
    } else if (qrType === 'wifi') {
        var ssid = $('#wifiName').val().trim();
        var password = $('#wifiPassword').val();
        var encryption = $('#wifiEncryption').val(); // Get the selected encryption type
        if (!ssid) {
            Swal.fire({
                title: '',
                text: 'Please enter WiFi name',
                icon: 'warning',
            })
            return;
        }
        // Construct the string based on selected encryption type
        textToEncode = `WIFI:S:${ssid};T:${encryption};P:${password};;`;
    }

    // Generate the QR Code
    var qrCode = new QRCode(document.getElementById("qrcode"), {
        text: textToEncode,
        width: 128,
        height: 128,
        colorDark: "#000000",
        colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });

    setTimeout(() => downloadQRCode(qrType), 500); // Wait for QR Code to be drawn
}

function downloadQRCode(qrType) {
    let qrCodeElement = document.getElementById('qrcode').querySelector('img') || document.getElementById('qrcode').querySelector('canvas');
    let fileName = qrType === 'app' ? 'QRCode_App.png' : 'QRCode_WiFi.png';

    if (qrCodeElement.tagName === 'CANVAS') {
        // If the QR Code is drawn on a canvas element
        let image = qrCodeElement.toDataURL("image/png").replace("image/png", "image/octet-stream");
        downloadImage(image, fileName);
    } else if (qrCodeElement.tagName === 'IMG') {
        // If the QR Code is an image
        downloadImage(qrCodeElement.src, fileName);
    }
}

function downloadImage(data, fileName) {
    var a = document.createElement('a');
    a.href = data;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}