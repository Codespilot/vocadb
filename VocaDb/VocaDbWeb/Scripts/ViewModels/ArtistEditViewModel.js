var vdb;
(function (vdb) {
    (function (viewModels) {
        var ArtistEditViewModel = (function () {
            function ArtistEditViewModel(webLinkCategories, data) {
                var _this = this;
                this.submit = function () {
                    _this.submitting(true);
                    return true;
                };
                this.submitting = ko.observable(false);
                this.webLinks = new vdb.viewModels.WebLinksEditViewModel(data.webLinks, webLinkCategories);
            }
            return ArtistEditViewModel;
        })();
        viewModels.ArtistEditViewModel = ArtistEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
