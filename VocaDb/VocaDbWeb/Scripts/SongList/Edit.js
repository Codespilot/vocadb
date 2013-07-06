
var Song = function (data) {

	this.SongInListId = data.SongInListId;
	this.Notes = ko.observable(data.Notes);
	this.Order = ko.observable(data.Order);
	this.SongId = data.SongId;
	this.SongName = data.SongName;
	this.SongAdditionalNames = data.SongAdditionalNames;
	this.SongArtistString = data.SongArtistString;

};

function SongListViewModel(data) {

	var self = this;

	this.Id = data.Id;
	this.CurrentName = data.Name;
	this.Name = ko.observable(data.Name);
	this.Description = ko.observable(data.Description);
	this.FeaturedCategory = ko.observable(data.FeaturedCategory);
	this.SongLinks = ko.observableArray([]);

	var mappedSongs = $.map(data.SongLinks, function (item) { return new Song(item); });
	this.SongLinks(mappedSongs);

	function songListChanged() {

		var track = 1;

		$("tr.trackRow").each(function () {

			var songLink = ko.dataFor(this);
			songLink.Order(track);
			track++;

		});

	}

	$("#songsTableBody").sortable({
		update: function (event, ui) {
			songListChanged();
		}
	});

	function acceptSongSelection(songId) {

		if (!isNullOrWhiteSpace(songId)) {
			$.post("../../SongList/AddSong", { songId: songId }, songAdded);
		}

	}

	function songAdded(row) {

		self.SongLinks.push(new Song(row));
		songListChanged();

	}

	this.removeSong = function (songLink) {
		self.SongLinks.remove(songLink);
		songListChanged();
	};

	this.save = function () {
		ko.utils.postJson(location.href, { model: ko.toJS(self) });
	};
	
	this.songSearchParams = {
		acceptSelection: acceptSongSelection,
	};

};

function initPage(listId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });

	$.getJSON("/SongList/Data", { id: listId }, function (songListData) {
		var viewModel = new SongListViewModel(songListData);
		ko.applyBindings(viewModel);
		$("#songListForm").validate({ submitHandler: function () { viewModel.save(); } });
	});

}