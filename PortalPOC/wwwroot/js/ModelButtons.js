document.body.addEventListener('click', function (event) {
    // Check if the clicked element is the "createButton"
    if (event.target.id === "createButton") {
        // Create an object to store form data
        var formDataObject = {};

        // Loop through form elements and populate formDataObject
        var formElements = document.getElementById("createForm").elements;
        for (var i = 0; i < formElements.length; i++) {
            var element = formElements[i];
            if (element.name) {
                formDataObject[element.name] = element.value;
            }
        }

        // Perform an AJAX request using formDataObject
        fetch('/GenericTable/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formDataObject)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response as needed
                console.log(data);
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }
});