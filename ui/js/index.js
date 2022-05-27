function createRequest() {
    if (validateYouTubeUrl()) {
        let loader = `<div class="loaderBlock">
        <div class="loader"></div>
        </div>Fetching...`;
        document.getElementById('videoBlock').innerHTML = loader;

        fetch('https://localhost:44397/api/convert', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                'url': $('#videoLink').val()
            })
        }).then(response => response.json()).then(function(data) {


            let videoBlockContent = `<div id="videoTitle">${data.details.title}</div>
    <div id="videoThumbnail">
        <img src="${data.details.thumbnailURL}" alt="${data.details.title}" class="thumbnail" />
    </div>
    <div id="videoPublished">${data.details.publishDate}</div>
    <details>
        <summary>
            Description
        </summary>
        <div id="videDescription">${data.details.description}</div>
    </details>
    <div class="downloadButtons">
    <a href="${data.audioURL}" title="${data.details.title}" class="btn" role="button" download="${data.details.title}.mp3"><i class="fa fa-download"></i>Download MP3</a>
    <a href="${data.videoURL}" title="${data.details.title}" class="btn" role="button" download="${data.details.title}.mp4"><i class="fa fa-download"></i>Download MP3</a>

    </div>`;
            document.getElementById('videoBlock').innerHTML = videoBlockContent;
        });
    }



};

function printMessage(message) {
    document.getElementById('videoBlock').innerHTML = message;

};

function validateYouTubeUrl() {
    var url = $('#videoLink').val();
    if (url != undefined || url != '') {
        var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=|\?v=)([^#\&\?]*).*/;
        var match = url.match(regExp);
        if (match && match[2].length == 11) {
            return true;

        } else {
            return false;
        }
    }
};