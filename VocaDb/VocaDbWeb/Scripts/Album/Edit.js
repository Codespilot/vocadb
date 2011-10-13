
function onChangeLang(event) {

	var select = event.target;

	var id = getId(select);
	var val = $(select).val();

	$.post("../../Album/EditNameLanguage", { nameId: id, nameLanguage: val });

}

function initPage(albumId) {

	$("input.nameEdit").live("change", function () {

		var id = getId(this);
		var val = $(this).val();

		$.post("../../Album/EditNameValue", { nameId: id, nameVal: val });

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

		$.post("../../Album/DeleteName", { nameId: id });

		$("tr#nameRow_" + id).remove();

	});

	function createLanguageDropDown(nameId, nameLang) {

		var languages = eval("[\"Japanese\",\"Romaji\",\"English\"]");

		var dropDown = document.createElement("select");
		$(dropDown).attr("id", "nameLanguage_" + nameId);

		$(languages).each(function () {

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

		var aId = albumId;
		var newNameVal = $("input#nameEdit_new").val();
		var newLangId = $("select#nameLanguage_new").val();

		$.post("../../Album/CreateName", { albumId: aId, nameVal: newNameVal, language: newLangId }, function (name) {

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

		$.post("../../Album/EditWebLinkDescription", { linkId: id, description: val });

	});

	$("input.webLinkUrl").live("change", function () {

		var id = getId(this);
		var val = $(this).val();

		$.post("../../Album/EditWebLinkUrl", { linkId: id, url: val });

	});

	$("input.webLinkDelete").live("click", function () {

		var id = getId(this);

		$.post("../../Album/DeleteWebLink", { linkId: id });

		$("tr#webLinkRow_" + id).remove();

	});

	$("input#webLinkAdd").click(function () {

		var aId = albumId;
		var newDescription = $("input#webLinkDescription_new").val();
		var newUrl = $("input#webLinkUrl_new").val();

		$.post("../../Album/CreateWebLink", { albumId: albumId, description: newDescription, url: newUrl }, function (link) {

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

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length < 3) {

			$(artistList).empty();

			if (findTerm.length > 0) {
				addOption(artistList, "", "Create new artist named '" + findTerm + "'");
			}

			return;
		}

		$.post("../../Artist/FindJson", { term: findTerm, artistTypes: "Performer,Producer,Circle" }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

			addOption(artistList, "", "Create new artist named '" + findTerm + "'");

		});

	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0)
			return;

		var artistId = $(artistList).val();

		if (artistId == "") {
			$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: findTerm }, artistAdded);
		} else {
			$.post("../../Album/AddExistingArtist", { albumId: albumId, artistId: artistId }, artistAdded);
		}

	});

	function artistAdded(row) {

		var addRow = $("#artistRow_new");
		addRow.before(row);
		$("input#artistAddName").val("");
		$("#artistAddList").empty();

	}

	$("input.artistRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteArtistForAlbum", { artistForAlbumId: id }, function () {

			$("tr#artistRow_" + id).remove();

		});

	});

	$("input#songAddName").keyup(function () {

		var findTerm = $(this).val();
		var songList = $("#songAddList");

		if (findTerm.length < 3) {

			$(songList).empty();

			if (findTerm.length > 0) {
				addOption(songList, "", "Create new song named '" + findTerm + "'");
			}

			return;
		}

		$.post("../../Song/FindJsonByName", { term: findTerm }, function (results) {

			$(songList).empty();

			$(results).each(function () {

				addOption(songList, this.Id, this.Name);

			});

			addOption(songList, "", "Create new song named '" + findTerm + "'");

		});

	});

	$("#songAddBtn").click(function () {

		var findTerm = $("input#songAddName").val();
		var songList = $("#songAddList");

		if (findTerm.length == 0)
			return;

		var songId = $(songList).val();

		if (songId == "") {
			$.post("../../Album/AddNewSong", { albumId: albumId, newSongName: findTerm }, songAdded);
		} else {
			$.post("../../Album/AddExistingSong", { albumId: albumId, songId: songId }, songAdded);
		}

	});

	function songAdded(row) {

		var addRow = $("#songRow_new");
		addRow.before(row);
		$("input#songAddName").val("");
		$("#songAddList").empty();

	}

	$("input.songMoveUp").live("click", function () {

		var id = getId(this);
		$.post("../../Album/MoveSongInAlbumUp", { songInAlbumId: id }, songListChanged);

	});

	$("input.songMoveDown").live("click", function () {

		var id = getId(this);
		$.post("../../Album/MoveSongInAlbumDown", { songInAlbumId: id }, songListChanged);

	});

	$("input.songRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteSongInAlbum", { songInAlbumId: id }, function (songList) {

			$("tr#songRow_" + id).remove();
			songListChanged(songList);

		});

	});

	function songListChanged(songList) {

		for (var i = 0; i < songList.length; ++i) {

			var item = songList[i];

			var id = item.Id;
			var newTrackNum = item.TrackNumber;
			var trackNumElem = $("#songTrackNumber_" + id);
			var oldTrackNum = $(trackNumElem).val();

			if (newTrackNum != oldTrackNum) {

				var row = $("tr#songRow_" + id);

				$(trackNumElem).val(newTrackNum);

				var nextItem = (i < songList.length - 1 ? songList[i + 1] : null);
				var nextItemId = (nextItem != null ? nextItem.Id : null);
				var oldNextRow = $(row).next();
				var newNextRow = (nextItemId != null ? $("tr#songRow_" + nextItemId) : null);

				var nextRowId = getId(oldNextRow);

				if (nextItemId != nextRowId) {

					$(row).remove();

					if (newNextRow != null) {

						newNextRow.before(row);

					} else {

						var addRow = $("#songRow_new");
						addRow.before(row);

					}

				}

			}

		}

	}

}
