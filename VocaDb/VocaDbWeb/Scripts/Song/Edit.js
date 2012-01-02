
function initPage(songId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();

	initNamesList();
	initWebLinksList();

	$("#changeOriginalBtn").click(function () {

		$("#changeOriginalPanel").show();
		return false;

	});

	function acceptOriginalSong(artistId, term) {

		$("#changeOriginalPanel").hide();
		var newOriginalId = $("#changeOriginalId").val();

		$.post("../../Song/CreateSongLink", { songId: newOriginalId }, function (content) {
			$("#originalContent").html(content);
		});

	}

	var acceptNewOriginalBtn = $("#acceptNewOriginalBtn");
	var changeOriginalList = $("#changeOriginalList");
	var changeOriginalName = $("input#changeOriginalName");
	var changeOriginalId = $("#changeOriginalId");

	initEntrySearch(changeOriginalName, changeOriginalList, "Song", "../../Song/FindJsonByName",
		{
			acceptBtnElem: acceptNewOriginalBtn,
			acceptSelection: acceptOriginalSong,
			idElem: changeOriginalId,
			createOptionFirstRow: function (item) { return item.Name; },
			createOptionSecondRow: function (item) { return item.ArtistString; },
			createTitle: function (item) { return item.AdditionalNames; },
			extraQueryParams: { ignoredIds: JSON.stringify(songId) },
			height: 250
		});

	function acceptArtistSelection(artistId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../Song/AddExistingArtist", { songId: songId, artistId: artistId }, artistAdded);
		} else {
			//$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: term }, artistAdded);
		}

	}

	var artistAddList = $("#artistAddList");
	var artistAddName = $("input#artistAddName");
	var artistAddBtn = $("#artistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{
			allowCreateNew: false,
			acceptBtnElem: artistAddBtn,
			acceptSelection: acceptArtistSelection,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.AdditionalNames },
			extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer,Unknown" }
		});

	function artistAdded(row) {

		var artistsTable = $("#artistsTableBody");
		artistsTable.append(row);
		//$("input#artistAddName").val("");
		//$("#artistAddList").empty();

	}

	$("a.artistRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Song/DeleteArtistForSong", { artistForSongId: id }, function () {

			$("tr#artistRow_" + id).remove();

		});

		return false;

	});

	$("#pvAdd").click(function () {

		var service = $("#pvService_new").val();
		var pvUrl = $("#pvUrl_new").val();
		var pvType = $("#pvType_new").val();

		$("#pvUrl_new").val("");

		$.post("../../Song/CreatePVForSongByUrl", { songId: songId, pvUrl: pvUrl, type: pvType }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
				return;
			}

			var addRow = $("#pvTableBody");
			addRow.append(response.Result);

		});

		return false;

	});

	$("a.pvRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Song/DeletePVForSong", { pvForSongId: id }, function () {

			$("tr#pvRow_" + id).remove();

		});

		return false;

	});

	$("#lyricsAdd").click(function () {

		$.post("../../Song/CreateLyrics", null, function (html) {

			$("#lyricsAdd").before(html);

		});

	});

	$("a.deleteLyrics").live("click", function () {

		$(this).parent().remove();

	});

}