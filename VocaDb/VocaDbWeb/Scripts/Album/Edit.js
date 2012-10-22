
function showTrackPropertiesPopup(albumId, songId) {

	$("#multipleTrackPropertiesContent").html("");

	$.get("../../Album/TrackProperties", { albumId: albumId, songId: songId }, function (content) {

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

	$.post("../../Album/TrackProperties", { songId: songId, artistIds: artistIds }, function (artistString) {

		var trackRows = $("tr.trackRow:has(input[type='hidden'][class='songId'][value='" + songId + "'])");

		trackRows.each(function () {

			$(this).find("span.artistString").text(artistString);

		});

	});

	return false;

}

function showMultipleTrackPropertiesPopup(albumId) {

	$("#trackPropertiesContent").html("");

	$.get("../../Album/MultipleTrackProperties", { albumId: albumId }, function (content) {

		$("#multipleTrackPropertiesContent").html(content);

		$("input.artistSelection").button();

		$("#editMultipleTrackPropertiesPopup").dialog("open");

	});

	return false;

}

function addArtistsToSelectedTracks() {

	updateArtistsForMultipleTracks(true);

}

function removeArtistsFromSelectedTracks() {

	updateArtistsForMultipleTracks(false);

}

function updateArtistsForMultipleTracks(add) {

	$("#editMultipleTrackPropertiesPopup").dialog("close");

	var trackPropertiesRows = $("#multipleTrackPropertiesContent input.artistSelection:checked");
	var artistIds = new Array();

	$(trackPropertiesRows).each(function () {

		var id = $(this).parent().find("input.artistSelectionArtistId").val();
		artistIds.push(id);

	});

	var songRows = $("#tracksTable input.trackSelection:checked");
	var songIds = new Array();
	$(songRows).each(function () {
		var id = $(this).parent().parent().find("input.songId").val();
		songIds.push(id);
	});

	$.ajax({
		type: "POST",
		url: "../../Album/UpdateArtistsForMultipleTracks",
		dataType: "json",
		traditional: true,
		data: { songIds: songIds, artistIds: artistIds, add: add },
		success: function (artistStrings) {

			$(artistStrings).each(function () {

				var songId = this.Key;
				var artistString = this.Value;

				var trackRows = $("tr.trackRow:has(input[type='hidden'][class='songId'][value='" + songId + "'])");

				trackRows.each(function () {

					$(this).find("span.artistString").text(artistString);

				});

			});

		}
	});

	return false;

}

function songListChanged() {

	var track = 1;
	var disc = 1;

	$("tr.trackRow").each(function () {

		if ($(this).find(".nextDiscCheck").is(":checked")) {
			disc++;
			track = 1;
		}

		$(this).find(".songDiscNumberField").val(disc);
		$(this).find(".songDiscNumber").html(disc);
		$(this).find(".songTrackNumberField").val(track);
		$(this).find(".songTrackNumber").html(track);
		track++;

	});

}

function initPage(albumId, discType) {
	
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$(".helpToolTip").tooltip();
	$("#pvLoader")
		.ajaxStart(function() { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#releaseEvent").autocomplete({
		source: "../../Album/FindReleaseEvents"
	});

	$(".isSupportCheck").live("change", function () {

		var id = getId(this);
		var isSupport = $(this).is(":checked");
		$.post("../../Album/UpdateArtistForAlbumIsSupport", { artistForAlbumId: id, isSupport: isSupport });

	});

	$("#editArtistRolesPopup").dialog({ autoOpen: false, width: 550, modal: true, buttons: { "Save": function () {

		var artistId = $("#rolesArtistId").val();
		var checkedRoles = $("#editArtistRolesPopup input.artistRoleCheck:checked").map(function () {
			return $(this).attr("id").split("_")[1];
		}).toArray();

		var idField = $("#artistsTableBody input.artistId[value='" + artistId + "']");
		var row = idField.parent().parent();
		row.find("input.artistRoles").val(checkedRoles.join(","));
		row.find("div.artistRolesList").html(checkedRoles.join("<br />"));

		$.ajax({
			type: "POST",
			url: "../../Album/UpdateArtistForAlbumRoles",
			dataType: "json",
			traditional: true,
			data: { artistForAlbumId: artistId, roles: checkedRoles }
		});

		$("#editArtistRolesPopup").dialog("close");

	} 
	}
	});

	$("input.artistRoleCheck").button();

	$("a.artistRolesEdit").live("click", function () {

		var row = $(this).parent().parent();

		var id = row.find("input.artistId").val();
		$("#rolesArtistId").val(id);

		var roles = row.find("input.artistRoles").val().split(",");
		$("#editArtistRolesPopup input.artistRoleCheck").each(function () {
			$(this).removeAttr("checked");
			$(this).button("refresh");
		});

		$(roles).each(function () {
			var check = $("#editArtistRolesPopup #artistRole_" + this.trim());
			$(check).attr("checked", "checked");
			$(check).button("refresh");
		});

		$("#editArtistRolesPopup").dialog("open");

		return false;

	});

	$("#tracksTableBody").sortable({
		update: function (event, ui) {
			songListChanged();
		}
	});

	$("#editTrackPropertiesPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: { "Save": saveTrackProperties } });
	$("#editMultipleTrackPropertiesPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: {
		"Add to tracks": addArtistsToSelectedTracks, "Remove from tracks": removeArtistsFromSelectedTracks
	} });

	$(".nextDiscCheck").live("click", function () {
		songListChanged();
	});

	initNamesList();
	initWebLinksList();

	function acceptArtistSelection(artistId, term) {

		if (isNullOrWhiteSpace(artistId)) {
			$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: term }, artistAdded);
		} else {
			$.post("../../Album/AddExistingArtist", { albumId: albumId, artistId: artistId }, artistAdded);
		}

	}

	var artistAddList = $("#artistAddList");
	var artistAddName = $("input#artistAddName");
	var artistAddBtn = $("#artistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{ 
			allowCreateNew: true, 
			acceptBtnElem: artistAddBtn, 
			acceptSelection: acceptArtistSelection,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.AdditionalNames }
		});

	function artistAdded(row) {

		var artistsTable = $("#artistsTableBody");
		artistsTable.append(row);

	}

	$("a.artistRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteArtistForAlbum", { artistForAlbumId: id }, function () {

			$("tr#artistRow_" + id).remove();

		});

		return false;

	});

	$("#selectAllTracksCheck").change(function () {

		var checked = $("#selectAllTracksCheck").is(':checked');
		$("input.trackSelection").attr('checked', checked);

	});

	$("#editSelectedTracksLink").click(function () {

		showMultipleTrackPropertiesPopup(albumId);
		return false;

	});

	function acceptSongSelection(songId, term) {

		if (isNullOrWhiteSpace(songId)) {
			$.post("../../Album/AddNewSong", { albumId: albumId, newSongName: term }, songAdded);
		} else {
			$.post("../../Album/AddExistingSong", { albumId: albumId, songId: songId }, songAdded);
		}

	}

	var songAddList = $("#songAddList");
	var songAddName = $("input#songAddName");
	var songAddBtn = $("#songAddAcceptBtn");
	var songTypes = "Unspecified,Original,Remaster,Remix,Cover,Mashup,Other" + (discType == "Video" ? ",MusicPV,DramaPV" : "");

	initEntrySearch(songAddName, songAddList, "Song", "../../Song/FindJsonByName",
		{
			allowCreateNew: true,
			acceptBtnElem: songAddBtn,
			acceptSelection: acceptSongSelection,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.SongType + ")"; },
			createOptionSecondRow: function (item) { return item.ArtistString; },
			createTitle: function (item) { return item.AdditionalNames; },
			extraQueryParams: { songTypes: songTypes }
		});

	function songAdded(row) {

		var tracksTable = $("#tracksTableBody");
		tracksTable.append(row);
		songListChanged();

	}

	$("a.songRemove").live("click", function () {

		$(this).parent().parent().remove();
		songListChanged();

		return false;

	});

	$(".editTrackProperties").live("click", function () {

		var songId = $(this).parent().parent().find("input.songId").val();

		if (songId == 0)
			return false;

		return showTrackPropertiesPopup(albumId, songId);

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

	$("#pvAdd").click(function () {

		var pvUrl = $("#pvUrl_new").val();

		$("#pvUrl_new").val("");

		$.post("../../Album/CreatePVForAlbumByUrl", { albumId: albumId, pvUrl: pvUrl }, function (response) {

			var result = handleGenericResponse(response);

			if (result == null)
				return;

			$("#pvTableBody").append(result);

		});

		return false;

	});

	$("a.pvRemove").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

}
