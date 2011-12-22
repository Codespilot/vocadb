
function initPage(artistId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();

	$("input.nameDelete").live("click", function () {

		$(this).parent().parent().remove();

	});

	$("input#nameAdd").click(function () {

		var aId = artistId;
		var newNameVal = $("input#nameEdit_new").val();
		var newLangId = $("select#nameLanguage_new").val();

		$.post("../../Shared/CreateName", { nameVal: newNameVal, language: newLangId }, function (row) {

			$("#nameRow_new").before(row);
			$("input#nameEdit_new").val("");

		});

	});

	$("input.webLinkDelete").live("click", function () {

		$(this).parent().parent().remove();

	});

	$("input#webLinkAdd").click(function () {

		var aId = artistId;
		var newDescription = $("input#webLinkDescription_new").val();
		var newUrl = $("input#webLinkUrl_new").val();

		$.post("../../Shared/CreateWebLink", { artistId: aId, description: newDescription, url: newUrl }, function (row) {

			$("#webLinkRow_new").before(row);
			$("input#webLinkDescription_new").val("");
			$("input#webLinkUrl_new").val("");

		});

	});

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
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.AdditionalNames },
			extraQueryParams: { artistTypes: "Label,Circle,OtherGroup" },
			height: 250
		});

	/*$("#groupAddBtn").click(function () {

		var groupId = $("#groupAddGroup").val();
		$.post("../../Artist/AddCircle", { artistId: artistId, circleId: groupId }, function (row) {

			$("#groupRow_new").before(row);

		});

	});*/

	$("input.groupRemove").live("click", function () {

		$(this).parent().parent().remove();		

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
	var albumAddBtn = $("#albumAddBtn");

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

		var addRow = $("#albumRow_new");
		addRow.before(row);

	}

	$("input.albumRemove").live("click", function () {

		$(this).parent().parent().remove();

	});

}
