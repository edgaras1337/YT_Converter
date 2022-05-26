function createRequest() {
    console.log('ds')
    var div = document.createElement('div');
    div.innerHTML = document.getElementById('videoLink').value;

    document.getElementById("converter").appendChild(div);
}