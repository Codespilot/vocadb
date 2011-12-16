
function initPage(songId) {

	var songList = $("#songList");
	var songName = $("input#songName");
	var songIdBox = $("#targetSongId");

	initEntrySearch(songName, songList, "Song", "../../Song/FindJsonByName",
		{
			idElem: songIdBox,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.ArtistString },
			createTitle: function (item) { return item.AdditionalNames }
		});

	$("#mergeBtn").click(function () {

		return confirm("Are you sure you want to merge the songs?");

	});

}