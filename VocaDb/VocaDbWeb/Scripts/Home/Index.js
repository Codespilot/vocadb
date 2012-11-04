
$(document).ready(function () {

    $('#songs-navi li').click(function (e) {
        e.preventDefault();

        $("#songs-navi li").removeClass("active");
        $(this).addClass("active");

        var id = getId(this);
        $.post("../Song/PVForSong", { pvId: id }, function (content) {
            $("#songPVPlayer").html(content);
            $("#songLink").attr("href", "../Song/Details/" + id);
        });

    });

    $("#songs-navi li").eq(0).addClass("active");

});