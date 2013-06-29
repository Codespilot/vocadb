var vdb;
(function (vdb) {
    (function (viewModels) {
        var ArtistEditViewModel = (function () {
            function ArtistEditViewModel(webLinkCategories, data) {
                this.webLinks = new viewModels.WebLinksEditViewModel(data.webLinks, webLinkCategories);
            }
            return ArtistEditViewModel;
        })();
        viewModels.ArtistEditViewModel = ArtistEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
