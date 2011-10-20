
function onChangeLang(event) {

	var select = event.target;

	var id = getId(select);
	var val = $(select).val();

	$.post("../../Artist/EditNameLanguage", { nameId: id, nameLanguage: val });
		
}

function initPage(artistId) {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();

	$("input.nameEdit").live("change", function () {

		var id = getId(this);
		var val = $(this).val();

		$.post("../../Artist/EditNameValue", { nameId: id, nameVal: val });

	});

	$("select.nameLanguage").change(onChangeLang);

	$("input.nameCopy").live("click", function () {

		var id = getId(this);
		var nameVal = $("input#nameEdit_" + id).val();
		var langId = $("select#nameLanguage_" + id).val();

		$("input#Name" + langId).val(nameVal);

	});

	$("input.nameDelete").live("click", function () {

		var id = getId(this);

		$.post("../../Artist/DeleteName", { nameId: id });

		$("tr#nameRow_" + id).remove();

	});

	$("input#nameAdd").click(function () {

		var aId = artistId;
		var newNameVal = $("input#nameEdit_new").val();
		var newLangId = $("select#nameLanguage_new").val();

		$.post("../../Artist/CreateName", { artistId: aId, nameVal: newNameVal, language: newLangId }, function (row) {

			$("#nameRow_new").before(row);
			$("input#nameEdit_new").val("");

		});

	});

	$("input.webLinkDescription").live("change", function () {

		var id = getId(this);
		var val = $(this).val();

		$.post("../../Artist/EditWebLinkDescription", { linkId: id, description: val });

	});

	$("input.webLinkUrl").live("change", function () {

		var id = getId(this);
		var val = $(this).val();

		$.post("../../Artist/EditWebLinkUrl", { linkId: id, url: val });

	});

	$("input.webLinkDelete").live("click", function () {

		var id = getId(this);

		$.post("../../Artist/DeleteWebLink", { linkId: id });

		$("tr#webLinkRow_" + id).remove();

	});

	$("input#webLinkAdd").click(function () {

		var aId = artistId;
		var newDescription = $("input#webLinkDescription_new").val();
		var newUrl = $("input#webLinkUrl_new").val();

		$.post("../../Artist/CreateWebLink", { artistId: aId, description: newDescription, url: newUrl }, function (link) {

			var row = document.createElement("tr");
			$(row).attr("id", "webLinkRow_" + link.Id);
			$("#webLinkRow_new").before(row);
			$(row).append("<td><input maxlength=\"512\" type=\"text\" class=\"webLinkDescription\" id=\"webLinkDescription_" + link.Id + "\" value=\"" + link.Description + "\" />");
			$(row).append("<td><input maxlength=\"512\" type=\"text\" class=\"webLinkUrl\" size=\"50\" id=\"webLinkUrl_" + link.Id + "\" value=\"" + link.Url + "\" />");

			var actionCell = document.createElement("td");
			$(actionCell).append("<input type=\"button\" class=\"webLinkDelete\" id=\"webLinkDelete_" + link.Id + "\" value=\"Delete\" />");
			$(row).append(actionCell);

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

		if (findTerm.length == 0) {

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

	$("#albumAddBtn").click(function () {

		var findTerm = $("input#albumAddName").val();
		var albumList = $("#albumAddList");

		if (findTerm.length == 0)
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

		var id = getId(this);
		$.post("../../Album/DeleteArtistForAlbum", { artistForAlbumId: id }, function () {

			$("tr#albumRow_" + id).remove();

		});

	});

}
