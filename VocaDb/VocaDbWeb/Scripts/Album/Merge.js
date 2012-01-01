
function initPage(albumId) {

	var albumList = $("#albumList");
	var albumName = $("input#albumName");
	var targetAlbumId = $("#targetAlbumId");

	initEntrySearch(albumName, albumList, "Album", "../../Album/FindJson",
		{
			idElem: targetAlbumId,
			createOptionFirstRow: function (item) { return (item.Id != albumId ? item.Name : null); },
			createOptionSecondRow: function (item) { return item.ArtistString; },
			createTitle: function (item) { return item.AdditionalNames; }
		});	

	$("#mergeBtn").click(function () {

		var targetAlbumId = $("#targetAlbumId").val();

		if (targetAlbumId == null || targetAlbumId == "") {
			alert("Album must be selected!");
			return false;
		}

		return confirm("Are you sure you want to merge the albums?");

	});	

}