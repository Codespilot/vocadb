function padStr(i) {
	return (i < 10) ? "0" + i : "" + i;
}

var IPRule = function (data) {
	this.Address = ko.observable(data.Address);
	this.Created = data.Created;
	this.CreatedFormatted = data.Created.toDateString() + " " + padStr(data.Created.getHours()) + ":" + padStr(data.Created.getMinutes());
	this.Id = data.Id;
	this.Notes = ko.observable(data.Notes);
};

function ManageBansViewModel(data) {

    var self = this;

    this.rules = ko.observableArray(_.map(data, function (item) { return new IPRule(item); } ));
    this.newRule = ko.observable({ Address: ko.observable() });

    this.add = function () {
    	self.rules.push(new IPRule({ Address: self.newRule().Address(), Notes: "", Created: new Date() }));
    };

    this.remove = function (ban) {
        self.rules.remove(ban);
    };

    this.save = function () {
        var json = ko.toJS(self);
        ko.utils.postJson(location.href, json);
    };

}

function initPage(rules) {
    var viewModel = new ManageBansViewModel(rules);
    ko.applyBindings(viewModel);
}