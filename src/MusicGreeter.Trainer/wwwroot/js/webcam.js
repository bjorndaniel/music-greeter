function startVideo(src) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
            const video = document.getElementById(src);
            if ("srcObject" in video) {
                video.srcObject = stream;
            } else {
                video.src = window.URL.createObjectURL(stream);
            }
            video.onloadedmetadata = function (e) {
                video.play();
            };
        });
    }
}

function getFrame(src, dest) {
    try {
        const video = document.getElementById(src);
        const canvas = document.getElementById(dest);
        canvas.getContext('2d').drawImage(video, 0, 0, 320, 240);
    }
    catch {
        return false;
    }
    return true;
}