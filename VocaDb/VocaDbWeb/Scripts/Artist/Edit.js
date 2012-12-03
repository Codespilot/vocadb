
function initPage(artistId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$(".helpToolTip").qtip();

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
	var groupAddBtn = $("#groupAddAcceptBtn");

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

	$("#picAdd").click(function () {

		$.post("../../Shared/CreateEntryPictureFile", null, function (row) {

			$("#picturesTableBody").append(row);

		});

		return false;

	});

	$("a.picRemove").live("click", function () {

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
