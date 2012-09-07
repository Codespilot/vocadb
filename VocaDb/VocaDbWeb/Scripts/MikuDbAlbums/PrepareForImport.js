
function initPage() {

    function acceptArtistSelection(albumId, term) {

        $.post("../../Album/Name", { id: albumId }, function (name) {

            $("#mergedAlbumId").append("<option value='" + albumId + "'>" + name + "</option>");
            $("#mergedAlbumId").val(albumId);
            $("#updateAlbumBtn").click();

        });

    }

    var artistAddList = $("#albumSearchList");
    var artistAddName = $("input#albumSearchName");
    var artistAddBtn = $("#albumSearchAcceptBtn");

    initEntrySearch(artistAddName, artistAddList, "Album", "../../Album/FindJson",
		{
		    allowCreateNew: false,
		    acceptBtnElem: artistAddBtn,
		    acceptSelection: acceptArtistSelection,
            autoHide: true,
		    createOptionFirstRow: function (item) { return item.Name; },
		    createOptionSecondRow: function (item) { return item.AdditionalNames; }
		});

}