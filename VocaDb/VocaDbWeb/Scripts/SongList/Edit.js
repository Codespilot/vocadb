
function initPage(listId) {

	$("#tabs").tabs();

	var songAddList = $("#songAddList");
	var songAddName = $("input#songAddName");
	var songAddBtn = $("#songAddAcceptBtn");

	initEntrySearch(songAddName, songAddList, "Song", "../../Song/FindJsonByName",
		{
			allowCreateNew: false,
			acceptBtnElem: songAddBtn,
			acceptSelection: acceptSongSelection,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.ArtistString },
			createTitle: function (item) { return item.AdditionalNames }
		});

	function songAdded(row) {

		$("#songsTableBody").append(row);
		songListChanged();

	}

	$("a.songRemove").live("click", function () {

		$(this).parent().parent().remove();
		songListChanged();

		return false;

	});

}