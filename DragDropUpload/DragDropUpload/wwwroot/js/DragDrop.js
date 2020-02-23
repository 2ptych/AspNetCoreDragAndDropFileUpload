var dragDropArea = $("#DragDropArea");

function setupDragDropArea() {

    dragDropArea[0].ondragover = function () {
        dragDropArea.addClass("on-drag-over");
        return false;
    }

    dragDropArea[0].ondragleave = function () {
        dragDropArea.removeClass("on-drag-over");
        return false;
    }
    dragDropArea[0].ondrop = dropHandler;
}

function fileHandler(files) {
    initializeProgress();
    var formData = new FormData();
    var progressBar = $('#progress-bar');
    var xhr = new XMLHttpRequest();
    xhr.open('POST', "https://localhost:44357/upload", true);
    xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest')
    xhr.upload.onprogress = function (event) {
        var progress = Math.round((event.loaded / event.total) * 100);
        progressBar.attr('value', progress);
    };
    xhr.upload.onload = function () {
        $('#progress-bar').hide();
    };
    for (var i = 0; i < files.length; i++) {
        formData.append("files", files[i]);
    }
    xhr.send(formData);
}

function dropHandler (e) {
    e.preventDefault();
    e.stopPropagation();
    var dt = e.dataTransfer;
    var files = dt.files;
    dragDropArea.removeClass("on-drag-over");
    fileHandler(files);
}

function initializeProgress() {
    let progressBar = document.getElementById('progress-bar');
    progressBar.value = 0;
    $('#progress-bar').show();
}

$('#DragDropArea').on('change', function () {
    var dtTransf = event.dataTransfer;
    var files = dtTransf.files;
    var info = $('#info');
    info.text(files.length);
})

$(document).ready(
    function () {
        setupDragDropArea();
    }
);