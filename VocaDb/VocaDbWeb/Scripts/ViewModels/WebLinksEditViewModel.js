var vdb;
(function (vdb) {
    (function (viewModels) {
        var dc = vdb.dataContracts;

        var WebLinksEditViewModel = (function () {
            function WebLinksEditViewModel(webLinkContracts, categories) {
                var _this = this;
                this.categories = categories;
                this.webLinks = ko.observableArray(_.map(webLinkContracts, function (contract) {
                    return new viewModels.WebLinkEditViewModel(contract);
                }));

                this.add = function () {
                    _this.webLinks.push(new viewModels.WebLinkEditViewModel());
                };

                this.remove = function (webLink) {
                    _this.webLinks.remove(webLink);
                };
            }
            return WebLinksEditViewModel;
        })();
        viewModels.WebLinksEditViewModel = WebLinksEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
