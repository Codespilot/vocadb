function padStr(i) {
	return (i < 10) ? "0" + i : "" + i;
}

var IPRule = function (data) {
	this.address = ko.observable(data.address);
	this.created = data.created;
	this.createdFormatted = data.created.toDateString() + " " + padStr(data.created.getHours()) + ":" + padStr(data.created.getMinutes());
	this.id = data.id;
	this.notes = ko.observable(data.notes);
};

function ManageBansViewModel(data) {

    var self = this;

    this.rules = ko.observableArray(_.map(data, function (item) { return new IPRule(item); } ));
    this.newRule = ko.observable({ address: ko.observable() });
	this.bannedIPs = ko.observableArray([]);

    this.add = function () {
    	self.rules.push(new IPRule({ address: self.newRule().address(), notes: "", created: new Date() }));
    };

    this.remove = function (ban) {
        self.rules.remove(ban);
    };

    this.save = function () {
        var json = ko.toJS(self);
        ko.utils.postJson(location.href, json);
    };

	$.getJSON(vdb.functions.mapAbsoluteUrl("/Admin/BannedIPs"), null, function(result) {
		self.bannedIPs(result);
	});

}

function initPage(rules) {
    var viewModel = new ManageBansViewModel(rules);
    ko.applyBindings(viewModel);
}