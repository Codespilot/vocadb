
function showTrackPropertiesPopup(songInAlbumId) {

	$.get("../../Album/TrackProperties", { songInAlbumId: songInAlbumId }, function (content) {

		$("#trackPropertiesContent").html(content);

		$("input.artistSelection").button();

		$("#editTrackPropertiesPopup").dialog("open");

	});

	return false;

}

function saveTrackProperties() {

	$("#editTrackPropertiesPopup").dialog("close");

	var trackPropertiesRows = $("input.artistSelection:checked");
	var artistIds = "";

	$(trackPropertiesRows).each(function () {

		if ($(this).is(":checked"))
			artistIds += getId(this) + ",";

	});

	var songId = getId($(".trackProperties"));

	$.post("../../Album/TrackProperties", { songId: songId, artistIds: artistIds });

	return false;

}

function updateTrackList(albumId, event, ui) {

	var id = getId(ui.item);
	var prev = $(ui.item).prev();
	var prevId = getId(prev);

	$.post("../../Album/ReorderTrack", { songInAlbumId: id, prevTrackId: prevId }, function (songList) {
		songListChanged(songList);
	});	

}

function songListChanged(songList) {

	$(songList).each(function () {

		var trackNumElem = $("#songTrackNumber_" + this.Id);
		$(trackNumElem).html(this.TrackNumber);

	});

}

function initPage(albumId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$("#tracksTableBody").sortable({
		update: function (event, ui) {
			updateTrackList(albumId, event, ui) 
		}
	});

	$("#editTrackPropertiesPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: { "Save": saveTrackProperties } });

	$("input.nameDelete").live("click", function () {

		$(this).parent().parent().remove();

	});

	$("input#nameAdd").click(function () {

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

		var newDescription = $("input#webLinkDescription_new").val();
		var newUrl = $("input#webLinkUrl_new").val();

		$.post("../../Shared/CreateWebLink", { description: newDescription, url: newUrl }, function (row) {

			$("#webLinkRow_new").before(row);
			$("input#webLinkDescription_new").val("");
			$("input#webLinkUrl_new").val("");

		});

	});

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (isNullOrWhiteSpace(findTerm)) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

		});

	});

	$("input#artistAddName").bind("paste", function (e) {
		var elem = $(this);
		setTimeout(function () {
			$(elem).trigger("keyup");
		}, 0);
	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");

		if (isNullOrWhiteSpace(findTerm))
			return;

		var artistId = $(artistList).val();

		if (artistId == "") {
			//$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: findTerm }, artistAdded);
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

		if (isNullOrWhiteSpace(findTerm)) {

			$(songList).empty();
			return;

		}

		$.post("../../Song/FindJsonByName", { term: findTerm }, function (results) {

			$(songList).empty();

			$(results.Items).each(function () {

				addOption(songList, this.Id, this.Name + (this.ArtistString != "" ? " (by " + this.ArtistString + ")" : ""));

			});

			addOption(songList, "", "Create new song named '" + findTerm + "'");

		});

	});

	$("input#songAddName").bind("paste", function (e) {
		var elem = $(this);
		setTimeout(function () {
			$(elem).trigger("keyup");
		}, 0);
	});

	$("#songAddBtn").click(function () {

		var findTerm = $("input#songAddName").val();
		var songList = $("#songAddList");

		if (isNullOrWhiteSpace(findTerm))
			return;

		var songId = $(songList).val();

		if (songId == "") {
			$.post("../../Album/AddNewSong", { albumId: albumId, newSongName: findTerm }, songAdded);
		} else {
			$.post("../../Album/AddExistingSong", { albumId: albumId, songId: songId }, songAdded);
		}

	});

	function songAdded(row) {

		var tracksTable = $("#tracksTableBody");
		tracksTable.append(row);
		$("input#songAddName").val("");
		$("#songAddList").empty();

	}

	$("input.songRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteSongInAlbum", { songInAlbumId: id }, function (songList) {

			$("tr#songRow_" + id).remove();
			songListChanged(songList);

		});

	});

	$(".editTrackProperties").live("click", function () {

		var id = getId(this);

		return showTrackPropertiesPopup(id);

	});

	$("#pvAdd").click(function () {

		var service = $("#pvService_new").val();
		var pvUrl = $("#pvUrl_new").val();

		$("#pvUrl_new").val("");

		$.post("../../Album/CreatePVForAlbumByUrl", { albumId: albumId, pvUrl: pvUrl }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
				return;
			}

			var addRow = $("#pvRow_new");
			addRow.before(response.Result);

		});

	});

	$("input.pvRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeletePVForAlbum", { pvForAlbumId: id }, function () {

			$("tr#pvRow_" + id).remove();

		});

	});

}
