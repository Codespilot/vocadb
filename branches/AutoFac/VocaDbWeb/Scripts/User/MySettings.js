var WebLink = function (data) {

	var self = this;

	if (data) {

		this.Category = ko.observable(data.Category);
		this.Description = ko.observable(data.Description);
		this.Id = data.Id;
		this.Url = ko.observable(data.Url);

	} else {

		this.Category = ko.observable();
		this.Description = ko.observable();
		this.Url = ko.observable();

	}

	this.Url.subscribe(function (url) {

		if (!self.Description()) {

			var matcher = vdb.functions.matchWebLink(url);

			if (matcher)
				self.Description(matcher.desc);

		}

	});

};

function WebLinksViewModel(data) {

	var self = this;

	this.webLinks = ko.observableArray(_.map(data, function (item) { return new WebLink(item); }));

	this.add = function () {
		self.webLinks.push(new WebLink());
	};

	this.remove = function (webLink) {
		self.webLinks.remove(webLink);
	};

	this.save = function () {
		var json = ko.toJS(self);
		ko.utils.postJson(location.href, json);
	};

}

function initPage(webLinks) {

    $("#tabs").tabs();

    var viewModel = new WebLinksViewModel(webLinks);
    ko.applyBindings(viewModel);

}