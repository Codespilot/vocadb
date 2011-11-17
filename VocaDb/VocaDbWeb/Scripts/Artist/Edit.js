
function initPage(artistId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
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

	$("#groupAddBtn").click(function () {

		var groupId = $("#groupAddGroup").val();
		$.post("../../Artist/AddCircle", { artistId: artistId, circleId: groupId }, function (row) {

			$("#groupRow_new").before(row);

		});

	});

	$("input.groupRemove").live("click", function () {

		$(this).parent().parent().remove();		

	});

	$("input#albumAddName").keyup(function () {

		var findTerm = $(this).val();
		var albumList = $("#albumAddList");

		if (isNullOrWhiteSpace(findTerm)) {

			$(albumList).empty();
			return;

		}

		$.post("../../Album/FindJson", { term: findTerm }, function (results) {

			$(albumList).empty();

			$(results).each(function () {

				addOption(albumList, this.Id, this.Name);

			});

			addOption(albumList, "", "Create new album named '" + findTerm + "'");

		});

	});

	$("input#albumAddName").bind("paste", function (e) {
		var elem = $(this);
		setTimeout(function () {
			$(elem).trigger("keyup");
		}, 0);
	});

	$("#albumAddBtn").click(function () {

		var findTerm = $("input#albumAddName").val();
		var albumList = $("#albumAddList");

		if (isNullOrWhiteSpace(findTerm))
			return;

		var albumId = $(albumList).val();

		if (albumId == "") {
			$.post("../../Artist/AddNewAlbum", { artistId : artistId, newAlbumName: findTerm }, albumAdded);
		} else {
			$.post("../../Artist/AddExistingAlbum", { artistId: artistId, albumId: albumId }, albumAdded);
		}

	});

	function albumAdded(row) {

		var addRow = $("#albumRow_new");
		addRow.before(row);
		$("input#albumAddName").val("");
		$("#albumAddList").empty();

	}

	$("input.albumRemove").live("click", function () {

		$(this).parent().parent().remove();

		/*var id = getId(this);
		$.post("../../Album/DeleteArtistForAlbum", { artistForAlbumId: id }, function () {

		$("tr#albumRow_" + id).remove();

		});*/

	});

}
