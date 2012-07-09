
var SongList = function (data) {

	this.id = data.Id;
	this.currentName = data.Name;
	this.name = ko.observable(data.Name);
	this.description = ko.observable(data.Description);
	this.featuredCategory = ko.observable(data.FeaturedCategory);

};

function songListChanged() {

	var track = 1;

	$("tr.trackRow").each(function () {

		$(this).find(".songOrderField").val(track);
		$(this).find(".songOrder").html(track);
		track++;

	});

}

function initPage(listId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#songsTableBody").sortable({
		update: function (event, ui) {
			songListChanged();
		}
	});

	function acceptSongSelection(songId, term) {

		if (!isNullOrWhiteSpace(songId)) {
			$.post("../../SongList/AddSong", { songId: songId }, songAdded);
		}

	}

	var songAddList = $("#songAddList");
	var songAddName = $("input#songAddName");
	var songAddBtn = $("#songAddAcceptBtn");

	initEntrySearch(songAddName, songAddList, "Song", "../../Song/FindJsonByName",
		{
			allowCreateNew: false,
			acceptBtnElem: songAddBtn,
			acceptSelection: acceptSongSelection,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.SongType + ")" },
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