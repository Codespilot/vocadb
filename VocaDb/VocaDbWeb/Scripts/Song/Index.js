
function IndexViewModel(model) {

	var self = this;

	this.artistId = ko.observable(model.artistId);
	this.artistName = ko.observable(model.artistName);
	this.draftsOnly = ko.observable(model.draftsOnly);
	this.filter = ko.observable(model.filter);
	this.matchMode = ko.observable(model.matchMode);
	this.onlyWithPVs = ko.observable(model.onlyWithPVs);
	this.since = ko.observable(model.since);
	this.songType = ko.observable(model.songType);
	this.sort = ko.observable(model.sort);
	this.view = ko.observable(model.view);
	this.filterArtistName = ko.observable(model.artistName);
	this.songs = ko.observableArray([]);

	getArtistName();

	this.filterString = ko.computed(function () {

		var first = true;
		var str = "";
		var filterPre = vdb.resources.entryIndex.FilterPre + " ";

		function appendJoin() {

			if (first) {
				str = filterPre;
				first = false;
			} else {
				str += ", ";
			}

		}

		if (!model.artistId && !isNullOrWhiteSpace(model.filter)) {

			appendJoin();
			var f = model.filter;

			if (self.matchMode() == "Exact") {
				str += vdb.resources.entryIndex.ExactTitleFilter.replace("{0}", f);
			} else if (self.matchMode() == "StartsWith") {
				str += vdb.resources.entryIndex.StartsWithTitleFilter.replace("{0}", f);
			} else {
				str += vdb.resources.entryIndex.TitleFilter.replace("{0}", f);
			}

		}

		if (self.artistName()) {

			appendJoin();
			str += vdb.resources.song.ArtistFilter.replace("{0}", self.artistName());

		}

		if (model.songType && model.songType != 'Unspecified') {

			appendJoin();

			var songTypeName = vdb.resources.songTypes[model.songType];

			str += vdb.resources.song.SongTypeFilter.replace("{0}", songTypeName);

		}

		if (model.onlyWithPVs) {

			appendJoin();

			str += vdb.resources.song.OnlyWithPVsFilter;

		}

		if (model.since) {

			appendJoin();

			str += vdb.resources.song.SinceFilter.replace("{0}", model.since);

		}

		if (model.draftsOnly) {

			appendJoin();

			str += vdb.resources.entryIndex.DraftsOnlyFilter;

		}

		if (str)
			str += ".";

		return str;

	});

	this.hasFilter = ko.computed(function () {
		return self.filterString();
	});

	this.clearArtist = function () {

		self.artistId(undefined);
		self.filterArtistName(undefined);

	};

	function getArtistName() {

		if (!self.artistId()) {
			self.filterArtistName("");
			return;
		}

		var url = vdb.functions.mapUrl("/Artist/Info");
		$.post(url, { id: self.artistId() }, function (result) {
			self.artistName(result.Name);
			self.filterArtistName(result.Name);
		});

	}

	var findArtistsUrl = vdb.functions.mapUrl("/Artist/FindJson");

	$("#artistNameSearch").autocomplete({
		source: function (request, response) {
			$.post(findArtistsUrl, { term: request.term }, function (results) {
				var items = _.map(results.Items, function (item) {
					return { label: item.Name + " (" + item.ArtistType + ")", value: item.Id };
				});
				response(items);
			});
		},
		select: function (event, ui) {
			if (ui.item.value) {
				self.artistId(ui.item.value);
				self.filterArtistName(ui.item.label);
			}
		}
	});

	this.destroyPV = function (data) {
		if (data)
			data.html("");
	};

	this.previewPV = function(data) {

		if (data.preview()) {
			data.preview(false);
			data.song(null);
			return;
		}

		var songId = data.songId;
		$.post(vdb.functions.mapUrl("/Song/PVPlayerWithRating"), { songId: songId }, function(result) {
			data.html(result.pvPlayer);
			var userRepository = new vdb.repositories.UserRepository(vdb.values.hostAddress);
			var ratingButtonsViewModel = new vdb.viewModels.PVRatingButtonsViewModel(userRepository, result.song, function() {
				vdb.ui.showSuccessMessage(vdb.resources.song.ThanksForRating);
			});
			data.song(ratingButtonsViewModel);
			data.preview(true);
		});

	};

}

// Initializes and maintains song rating status for the HTML table.
ko.bindingHandlers.pvPreviewStatus = {
	init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

		var pvRows = $(element).find(".pvRow");
		var songsArray = valueAccessor();

		// Parse all rows and create child binding context for each of them
		var songItems = _.map(pvRows, function (pvRow) {
			var item = { songId: $(pvRow).data("entry-id"), preview: ko.observable(false), html: ko.observable(null), song: ko.observable(null) };
			var childBindingContext = bindingContext.createChildContext(item);
			ko.applyBindingsToDescendants(childBindingContext, pvRow);
			return item;
		});

		songsArray(songItems);

		return { controlsDescendantBindings: true };
	}
};

function initPage(model) {

	$("#createLink").button({ disabled: $("#createLink").hasClass("disabled"), icons: { primary: 'ui-icon-plusthick'} });
	$("#filterBox").tooltip();

	var viewModel = new IndexViewModel(model);
	ko.applyBindings(viewModel);

}

/*function previewPV(songId) {

	var elem = $(".pvPreview[data-entry-id='" + songId + "']");
	var buttonsElem = $(".pvRatingButtons[data-entry-id='" + songId + "']")[0];
	elem.html("");
	elem.toggle();
	var visible = elem.is(":visible");

	if (visible) {
		elem.parent().find(".previewSong").addClass("active");

		$.post(vdb.functions.mapUrl("/Song/PVPlayerWithRating"), { songId: songId }, function (result) {
			
			elem.html(result.pvPlayer);

			var userRepository = new vdb.repositories.UserRepository(vdb.values.hostAddress);
			var ratingButtonsViewModel = new vdb.viewModels.PVRatingButtonsViewModel(userRepository, result.song);
			var data = ko.dataFor(buttonsElem);
			if (data)
				data.song(ratingButtonsViewModel);
			else
				ko.applyBindings({ song: ko.observable(ratingButtonsViewModel) }, buttonsElem);

		});
	} else {
		elem.parent().find(".previewSong").removeClass("active");
		var data = ko.dataFor(buttonsElem);
		data.song(null);
	}

}*/