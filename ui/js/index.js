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

            mp3link = data.audioURL;
            let videoBlockContent = `
            <div id="videoTitle">${data.details.title}</div>
            <div id="videoThumbnail">
            <img src="${data.details.thumbnailURL}" alt="${data.details.title}" class="thumbnail" />
            <div id="videoPublished">${data.details.publishDate}</div>`;
            if(data.lyricsPageURL != null) {
                videoBlockContent += `<a href=${data.lyricsPageURL}>Lyrics</a>`
            }  
            videoBlockContent+=`
            <details>
                <summary>
                    Description
                </summary>
                <div id="videDescription">${data.details.description}</div>
            </details>
            <div class="downloadButtons">
            </div>`;

            let downloadAudioBtn = $(`<button class="btn"><i class="fa fa-download"></i>Download MP3</a></button>`).on("click", () => {
                download(data.audioURL, data.details.title, "mp3");
            });
            let downloadVideoBtn = $(`<button class="btn"><i class="fa fa-download"></i>Download MP4</a></button>`).on("click", () => {
                download(data.videoURL, data.details.title, "mp4");
            });


            document.getElementById('videoBlock').innerHTML = videoBlockContent;

            $('.downloadButtons').append(downloadAudioBtn);
            $('.downloadButtons').append(downloadVideoBtn);
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

function download(link, filename, ext) {
    console.log(mp3link);
    axios({
        url: link,
        method: 'GET',
        responseType: 'blob',
    }).then((response) => {
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', `${filename}.${ext}`);
        document.body.appendChild(link);
        link.click();
    });
}