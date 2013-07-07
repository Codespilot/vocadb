
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

	this.SongLinks.subscribe(function() {

		for (var track = 0; track < self.SongLinks().length; ++track) {
			self.SongLinks()[track].Order(track + 1);
		}

	});

	function acceptSongSelection(songId) {

		if (!isNullOrWhiteSpace(songId)) {
			$.post(vdb.functions.mapAbsoluteUrl("/Song/DataById"), { id: songId }, function (song) {
				var songInList = new Song({ SongInListId: 0, Order: 0, SongId: song.id, SongName: song.name, SongAdditionalNames: song.additionalNames, SongArtistString: song.artistString, Notes: "" });
				self.SongLinks.push(songInList);
			});
		}

	}

	this.removeSong = function (songLink) {
		self.SongLinks.remove(songLink);
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