
function IndexViewModel(model) {

	var self = this;

	this.draftsOnly = ko.observable(model.draftsOnly);
	this.filter = ko.observable(model.filter);
	this.matchMode = ko.observable(model.matchMode);
	this.onlyWithPVs = ko.observable(model.onlyWithPVs);
	this.since = ko.observable(model.since);
	this.songType = ko.observable(model.songType);
	this.sort = ko.observable(model.sort);
	this.view = ko.observable(model.view);

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

		if (!isNullOrWhiteSpace(self.filter())) {

			appendJoin();
			var f = self.filter();

			if (f.startsWith("artist:")) {
				str += vdb.resources.song.ArtistFilter;
			} else if (self.matchMode() == "Exact") {
				str += vdb.resources.entryIndex.ExactTitleFilter.replace("{0}", f);
			} else if (self.matchMode() == "StartsWith") {
				str += vdb.resources.entryIndex.StartsWithTitleFilter.replace("{0}", f);
			} else {
				str += vdb.resources.entryIndex.TitleFilter.replace("{0}", f);
			}

			first = false;

		}

		if (self.songType() && self.songType() != 'Unspecified') {

			appendJoin();

			var songTypeName = vdb.resources.songTypes[self.songType()];

			str += vdb.resources.song.SongTypeFilter.replace("{0}", songTypeName);

		}

		if (self.onlyWithPVs()) {

			appendJoin();

			str += vdb.resources.song.OnlyWithPVsFilter;

		}

		if (self.since()) {

			appendJoin();

			str += vdb.resources.song.SinceFilter.replace("{0}", self.since());

		}

		if (self.draftsOnly()) {

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

	this.noFilter = ko.computed(function () {
		return !self.hasFilter();
	});
}

function initPage(model) {

	$("#createLink").button({ disabled: $("#createLink").hasClass("disabled"), icons: { primary: 'ui-icon-plusthick'} });

	var viewModel = new IndexViewModel(model);
	ko.applyBindings(viewModel);

}