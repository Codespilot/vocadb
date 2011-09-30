
function onChangeLang(event) {

	var select = event.target;

	var id = getId(select);
	var val = $(select).val();

	$.post("../../Artist/EditNameLanguage", { nameId: id, nameLanguage: val });
		
}

function initPage(artistId) {

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

	function createLanguageDropDown(nameId, nameLang) {

		var languages = eval("[\"Japanese\",\"Romaji\",\"English\"]");

		var dropDown = document.createElement("select");
		$(dropDown).attr("id", "nameLanguage_" + nameId);

		$(languages).each(function() {

			/*var option = document.createElement("option"); //"<option value=\"" + this + "\">" + this + "</option>";
			$(option).val(this),
			$(option).text(this);

			$(dropDown).append(option);*/
			addOption(dropDown, this, this);

		});

		$(dropDown).change(onChangeLang);

		return dropDown;

	}

	$("input#nameAdd").click(function () {

		var aId = artistId;
		var newNameVal = $("input#nameEdit_new").val();
		var newLangId = $("select#nameLanguage_new").val();

		$.post("../../Artist/CreateName", { artistId: aId, nameVal: newNameVal, language: newLangId }, function (name) {

			var row = document.createElement("tr");
			$(row).attr("id", "nameRow_" + name.Id);
			$("#nameRow_new").before(row);
			$(row).append("<td><input maxlength=\"128\" type=\"text\" class=\"nameEdit\" id=\"nameEdit_" + name.Id + "\" value=\"" + name.Value + "\" />");

			var languageCell = document.createElement("td");
			var languageDropDown = createLanguageDropDown(name.Id, name.Language);
			$(languageDropDown).val(name.Language);
			$(languageCell).append(languageDropDown);
			$(row).append(languageCell);

			var actionCell = document.createElement("td");
			$(actionCell).append("<input type=\"button\" class=\"nameDelete\" id=\"nameDelete_" + name.Id + "\" value=\"Delete\" />");
			$(actionCell).append("<input type=\"button\" class=\"nameCopy\" id=\"nameCopy_" + name.Id + "\" value=\"Copy to primary\" />");
			$(row).append(actionCell);

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

	$("input#albumAddName").keyup(function () {

		var findTerm = $(this).val();
		var albumList = $("#albumAddList");

		if (findTerm.length < 3) {

			$(albumList).empty();

			if (findTerm.length > 0) {
				addOption(albumList, "", "Create new album named '" + findTerm + "'");
			}

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

	}

	$("input.albumRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteArtistForAlbum", { artistForAlbumId: id }, function () {

			$("tr#albumRow_" + id).remove();

		});

	});

}
