var IPRule = function (data) {
    this.Address = ko.observable(data.Address);
};

function ManageBansViewModel(data) {

    var self = this;

    this.rules = ko.observableArray(data);
    this.newRule = ko.observable({ Address: ko.observable() });

    this.add = function () {
        self.rules.push({ Address: self.newRule().Address() });
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