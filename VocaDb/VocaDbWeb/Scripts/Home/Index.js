
$(document).ready(function () {

    $('#songs-navi .item').click(function (e) {
        e.preventDefault();

        $("#songs-navi .item").removeClass("active");
        $(this).addClass("active");

        var id = getId(this);
        var songId = $(this).find(".songId").val();
        $.post("../Song/PVForSong", { pvId: id }, function (content) {
            $("#songPVPlayer").html(content);
            $("#songLink").attr("href", "../Song/Details/" + songId);
        });

    });

    $("#songs-navi .item").eq(0).addClass("active");
    $(".scrollable").scrollable({ vertical: true, mousewheel: true });

});