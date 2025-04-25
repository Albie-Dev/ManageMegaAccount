window.downloadFile = (fileName, base64Data) => {
    console.log(fileName);
    var link = document.createElement('a');
    link.href = 'data:application/octet-stream;base64,' + base64Data;
    link.download = fileName;
    console.log(link.download);
    link.click();
};


function moveToNext(elementId) {
    var nextElement = document.getElementById(elementId);
    if (nextElement) {
        nextElement.focus();
    }
}