
$(document).ready(function () {

    $('#songs-navi .item').click(function (e) {
        e.preventDefault();

        $("#songs-navi .item").removeClass("active");
        $(this).addClass("active");

        var item = this;
        var id = getId(this);
        var songId = $(this).find(".songId").val();
        $.post("../Song/PVForSong", { pvId: id }, function (content) {
            $("#songPVPlayer").html(content);
            $("#songLink").attr("href", "../Song/Details/" + songId);
            $("#songPreviewName").text($(item).find(".songName").text());
            $("#songPreviewArtists").text($(item).find(".songArtists").text());
        });

    });

    $("#songs-navi .item").eq(0).addClass("active");
    $(".scrollable").scrollable({ vertical: true, mousewheel: true, keyboard: false });

});