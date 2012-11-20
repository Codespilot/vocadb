
$(document).ready(function () {

    $('#songs-navi .item').click(function (e) {
        e.preventDefault();

        $("#songs-navi .item").removeClass("active");
        $(this).addClass("active");

        var songId = $(this).find(".songId").val();
        $.post("../Home/PVContent", { songId: songId }, function (content) {
            $("#songPreview").html(content);
        });

    });

    $("#songs-navi .item").eq(0).addClass("active");
    $(".scrollable").scrollable({ vertical: true, mousewheel: true, keyboard: false });

    function setRating(rating, callback) {

        var songId = $("#songPreview").find(".songId").val();

        $.post("../User/AddSongToFavorites", { songId: songId, rating: rating }, callback);

    }

    $("#addFavoriteLink").live("click", function () {

        setRating('Favorite', function () {

            $("#removeFavoriteLink").show();
            $("#ratingButtons").hide();
            vdb.ui.showSuccessMessage(vdb.resources.song.thanksForRating);

        });

        return false;

    });

    $("#addLikeLink").live("click", function () {

        setRating('Like', function () {

            $("#removeFavoriteLink").show();
            $("#ratingButtons").hide();
            vdb.ui.showSuccessMessage(vdb.resources.song.thanksForRating);

        });

        return false;

    });

    $("#removeFavoriteLink").live("click", function () {

        setRating('Nothing', function () {

            $("#ratingButtons").show();
            $("#removeFavoriteLink").hide();

        });

        return false;

    });

    function albumToolTip(img) {
        $(img).qtip({
            content: {
                text: 'Loading...',
                ajax: {
                    url: '/Album/PopupContent',
                    type: 'GET',
                    data: { id: $(img).data("entryId") }
                }
            },

        });
    }

    $("#newAlbums img").each(function () { albumToolTip(this); });
    $("#topAlbums img").each(function () { albumToolTip(this); });

});
