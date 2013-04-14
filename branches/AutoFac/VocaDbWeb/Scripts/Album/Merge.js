
function initPage(albumId) {

	function acceptTargetAlbum(targetAlbumId) {

		$.post(vdb.functions.mapUrl("/Album/Name"), { id: targetAlbumId }, function (content) {
			$("#targetAlbum").html("<a href='" + vdb.functions.mapUrl("/Album/Details/" + targetAlbumId) + "'>" + content + "</a>");
		});

	}

	var albumList = $("#albumList");
	var albumName = $("input#albumName");
	var targetAlbumId = $("#targetAlbumId");

	initEntrySearch(albumName, albumList, "Album", "../../Album/FindJson",
		{
			acceptSelection: acceptTargetAlbum,
			filter: function (item) { return item.Id != albumId; },
			idElem: targetAlbumId,
			createOptionFirstRow: function (item) { return item.Name; },
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