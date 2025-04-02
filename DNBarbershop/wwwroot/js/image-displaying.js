document.getElementById("imageFileInput").addEventListener("change", function (event) {
    var file = event.target.files[0]; // Get the selected file
    if (file) {
        var reader = new FileReader(); // Read the file
        reader.onload = function (e) {
            var imagePreview = document.getElementById("imagePreview");
            imagePreview.src = e.target.result; // Set preview image source
            imagePreview.style.display = "block"; // Make it visible
        };
        reader.readAsDataURL(file); // Convert to Base64 URL
    }
});