window.uploadFile = function (fileBuffer, fileName, token, tags, dotNetHelper) {
    const dotnetMethodName = 'UploadComplete';
    const dotnetProgressMethodName = 'UpdateProgress';
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

        xhr.upload.onprogress = function (event) {
            if (event.lengthComputable) {
                var progress = (event.loaded / event.total) * 100;
                dotNetHelper.invokeMethodAsync("UpdateProgress", progress);
            }
        };

        xhr.onload = function () {
            try {
                if (xhr.status == 200) {
                    dotNetHelper.invokeMethodAsync(dotnetMethodName, true, xhr.responseText);
                } else {
                    dotNetHelper.invokeMethodAsync(dotnetMethodName, false, `Status Code = ${xhr.status}. Error = ${xhr.responseText}`);
                }
            } catch (error) {
                dotNetHelper.invokeMethodAsync(dotnetMethodName, false, error);
            }
        };

        xhr.onerror = function () {
            try {
                dotNetHelper.invokeMethodAsync(dotnetMethodName, false, `Status Code = ${xhr.status}. Error = ${xhr.responseText}`);
            } catch (error) {
                dotNetHelper.invokeMethodAsync(dotnetMethodName, false, xhr.responseText);
            }
        };

        xhr.send(formData);
    } catch (error) {
        dotNetHelper.invokeMethodAsync(dotnetMethodName, false, xhr.responseText);
    }
};
