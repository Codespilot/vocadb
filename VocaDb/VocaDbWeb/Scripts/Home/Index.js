
$(document).ready(function () {

    $('#songs-navi li').click(function (e) {
        e.preventDefault();

        $("#songs-navi li").removeClass("active");
        $(this).addClass("active");

        var id = getId(this);
        $.post("../Song/PVForSong", { pvId: id }, function (content) {
            $("#songPVPlayer").html(content);
        });

    });

    $("#songs-navi li").eq(0).addClass("active");

});