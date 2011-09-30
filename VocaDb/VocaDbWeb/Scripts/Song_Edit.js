
function initPage(songId) {

	$("#tabs").tabs();	

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length < 3) {

			$(artistList).empty();

			if (findTerm.length > 0) {
				addOption(artistList, null, "Create new artist named '" + findTerm + "'");
			}

			return;
		}

		$.post("../../Artist/FindJson", { term: findTerm }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

			addOption(artistList, null, "Create new artist named '" + findTerm + "'");

		});

	});

	$("#artistAddBtn").click(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0)
			return;

		var artistId = $(artistList).val();



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