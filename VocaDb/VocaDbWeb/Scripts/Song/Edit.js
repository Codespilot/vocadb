
function initPage(songId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();

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

		var lang = $("#lyricsLanguage_new").val();
		var source = $("#lyricsSource_new").val();
		var value = $("#lyricsValue_new").val();

		$.post("../../Song/CreateLyrics", { }, function (html) {

			$("#lyricsAdd").before(html);

		});

	});

	$(".deleteLyrics").live("click", function () {

		$(this).parent().remove();

	});

}