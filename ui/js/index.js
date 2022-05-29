function createRequest() {
  $("#linkInput").removeAttr("style");
  if (validateYouTubeUrl()) {
    let loader = `<div class="loaderBlock">
        <div class="loader"></div>
        </div><span class="text-center">Fetching...</span>`;
    document.getElementById("videoBlock").innerHTML = loader;

    fetch(
      "https://localhost:8001/api/convert?url=" +
        encodeURI($("#videoLink").val()),
      {
        method: "POST",
        headers: {
          accept: "*/*",
          "content-type": "application/x-www-form-urlencoded",
        },
      }
    )
      .then((response) => response.json())
      .then(function (data) {
        let videoBlockContent = `
            <div id="videoTitle">${data.details.title}</div>
            <div id="videoThumbnail">
            <img src="${data.details.thumbnailURL}" alt="${data.details.title}" class="thumbnail" />
            <div id="videoPublished"><i class="fa-solid fa-calendar-plus"></i> ${data.details.publishDate}</div>`;
        if (data.lyricsPageURL != null) {
          videoBlockContent += `<a href=${data.lyricsPageURL} target="_blank">Lyrics</a>`;
        }
        videoBlockContent += `
            <details>
                <summary>
                    Description
                </summary>
                <div id="videDescription">${data.details.description}</div>
            </details>
            <div class="downloadButtons">
            </div>`;

        let downloadAudioBtn = $(
          `<button class="btn"><i class="fa fa-download"></i> Download MP3</a></button>`
        ).on("click", () => {
          download(data.audioURL, data.details.title, "mp3");
        });
        let downloadVideoBtn = $(
          `<button class="btn"><i class="fa fa-download"></i> Download MP4</a></button>`
        ).on("click", () => {
          download(data.videoURL, data.details.title, "mp4");
        });

        document.getElementById("videoBlock").innerHTML = videoBlockContent;

        $(".downloadButtons").append(downloadAudioBtn);
        $(".downloadButtons").append(downloadVideoBtn);
      });
  } else {
    $("#linkInput").css("border-color", "#e60c0c");
    document.getElementById(
      "videoBlock"
    ).innerHTML = `<span class="errorMsg">Enter a valid URL</span>`;
  }
}

function printMessage(message) {
  document.getElementById("videoBlock").innerHTML = message;
}

function validateYouTubeUrl() {
  var url = $("#videoLink").val();
  if (url != undefined || url != "") {
    var regExp =
      /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=|\?v=)([^#\&\?]*).*/;
    var match = url.match(regExp);
    if (match && match[2].length == 11) {
      return true;
    } else {
      return false;
    }
  }
}

function download(link, filename, ext) {
  axios({
    url: link,
    method: "GET",
    responseType: "blob",
  }).then((response) => {
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", `${filename}.${ext}`);
    document.body.appendChild(link);
    link.click();
  });
}
