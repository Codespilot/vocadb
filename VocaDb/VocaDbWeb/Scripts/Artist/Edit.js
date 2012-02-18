
function initPage(artistId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$("#statusHelp").tooltip();

	initNamesList();
	initWebLinksList();

	function acceptGroupSelection(groupId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../Artist/AddCircle", { artistId: artistId, circleId: groupId }, function (row) {

				$("#groupTableBody").append(row);

			});
		}

	}

	var groupAddList = $("#groupAddList");
	var groupAddName = $("input#groupAddName");
	var groupAddBtn = $("#groupAddBtn");

	initEntrySearch(groupAddName, groupAddList, "Artist", "../../Artist/FindJson",
		{
			allowCreateNew: false,
			acceptBtnElem: groupAddBtn,
			acceptSelection: acceptGroupSelection,
			autoHide: true,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.AdditionalNames },
			extraQueryParams: { artistTypes: "Label,Circle,OtherGroup" },
			height: 200,
			width: 350
		});

	$("a.groupRemove").live("click", function () {

		$(this).parent().parent().remove();		
		return false;

	});

	function acceptAlbumSelection(albumId, term) {

		if (isNullOrWhiteSpace(albumId)) {
			$.post("../../Artist/AddNewAlbum", { artistId: artistId, newAlbumName: term }, albumAdded);
		} else {
			$.post("../../Artist/AddExistingAlbum", { artistId: artistId, albumId: albumId }, albumAdded);
		}
		
	}

	var albumAddList = $("#albumAddList");
	var albumAddName = $("input#albumAddName");
	var albumAddBtn = $("#albumAddAcceptBtn");

	initEntrySearch(albumAddName, albumAddList, "Album", "../../Album/FindJson",
		{
			allowCreateNew: true,
			acceptBtnElem: albumAddBtn,
			acceptSelection: acceptAlbumSelection,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.ArtistString },
			createTitle: function (item) { return item.AdditionalNames }
		});

	function albumAdded(row) {

		$("#albumTableBody").append(row);

	}

	$("a.albumRemove").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

}
