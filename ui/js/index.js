function createRequest() {
    console.log('ds')
    var div = document.createElement('div');
    div.innerHTML = document.getElementById('videoLink').value;

    document.getElementById("converter").appendChild(div);
}

function printMessage(message) {
    document.getElementById('videoBlock').innerHTML = message;
    
}

function validateYouTubeUrl() {    
    var url = $('#videoLink').val();
    if (url != undefined || url != '') {        
        var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=|\?v=)([^#\&\?]*).*/;
        var match = url.match(regExp);
        if (match && match[2].length == 11) {
            printMessage("LINK IS VALID");
           

        } else {
            printMessage("LINK IS NOT VALID");
        }
    }
}