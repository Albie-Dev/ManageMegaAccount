window.uploadFile = function (fileBuffer, fileName, token, tags, dotNetHelper) {
    try {
        var blob = new Blob([fileBuffer], { type: 'application/octet-stream' });
        var file = new File([blob], fileName, { type: 'application/octet-stream' });
        var formData = new FormData();
        formData.append("file", file);
        formData.append("fileName", fileName);
        formData.append("token", token);
        formData.append('useUniqueFileName', 'true');
        formData.append("tags", tags);
        

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "https://upload.imagekit.io/api/v2/files/upload", true);

        console.log("Uploading file: ", fileName, " with token: ", token);

        xhr.upload.onprogress = function (event) {
            if (event.lengthComputable) {
                var progress = (event.loaded / event.total) * 100;
                dotNetHelper.invokeMethodAsync("UpdateProgress", progress);
            }
        };

        xhr.onload = function () {
            try {
                if (xhr.status == 200) {
                    var response = JSON.parse(xhr.responseText);
                    console.log(response);
                    dotNetHelper.invokeMethodAsync("UploadComplete", true, xhr.responseText);
                } else {
                    // Log failed status and response for debugging
                    console.error("Upload failed with status: " + xhr.status);
                    console.error("Response: ", xhr.responseText);
                    dotNetHelper.invokeMethodAsync("UploadComplete", false, null);
                }
            } catch (error) {
                // Catch any parsing errors and log them
                console.error("Error parsing response: ", error);
                dotNetHelper.invokeMethodAsync("UploadComplete", false, null);
            }
        };

        xhr.onerror = function () {
            try {
                console.error("Request failed with status: " + xhr.status);
                console.error("Error response: ", xhr.responseText);
                dotNetHelper.invokeMethodAsync("UploadComplete", false, null);
            } catch (error) {
                console.error("Error in onerror handler: ", error);
                dotNetHelper.invokeMethodAsync("UploadComplete", false, null);
            }
        };

        xhr.send(formData);
    } catch (error) {
        // Catch any general errors during the function execution
        console.error("Error in file upload: ", error);
        dotNetHelper.invokeMethodAsync("UploadComplete", false, null);
    }
};
