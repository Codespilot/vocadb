
function initPage(songId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
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

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm, artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer,Unknown" }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

			//addOption(artistList, "", "Create new artist named '" + findTerm + "'");

		});

	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0)
			return;

		var artistId = $(artistList).val();

		if (artistId == "") {
			//$.post("../../Song/AddNewArtist", { songId: songId, newArtistName: findTerm }, artistAdded);
		} else {
			$.post("../../Song/AddExistingArtist", { songId: songId, artistId: artistId }, artistAdded);
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
		$.post("../../Song/DeleteArtistForSong", { artistForSongId: id }, function () {

			$("tr#artistRow_" + id).remove();

		});

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

			var addRow = $("#pvRow_new");
			addRow.before(response.Result);

		});

	});

	$("input.pvRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Song/DeletePVForSong", { pvForSongId: id }, function () {

			$("tr#pvRow_" + id).remove();

		});
		
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