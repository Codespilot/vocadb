var vdb;
(function (vdb) {
    (function (viewModels) {
        var AlbumEditViewModel = (function () {
            function AlbumEditViewModel(webLinkCategories, data) {
                this.webLinks = new viewModels.WebLinksEditViewModel(data.webLinks, webLinkCategories);
            }
            return AlbumEditViewModel;
        })();
        viewModels.AlbumEditViewModel = AlbumEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
