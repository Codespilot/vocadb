
function initPage(songId) {

	$("#tabs").tabs();	

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

		$.post("../../Artist/FindJson", { term: findTerm }, function (results) {

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
			$.post("../../Song/AddNewArtist", { songId: songId, newArtistName: findTerm }, artistAdded);
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

	$("#lyricsAdd").click(function () {

		var lang = $("#lyricsLanguage_new").val();
		var source = $("#lyricsSource_new").val();
		var value = $("#lyricsValue_new").val();

		$.post("../../Song/CreateLyrics", { songId: songId, languageSelection: lang, source: source, value: value }, function (html) {

			$("#lyrics_new").before(html);

		});

	});

}