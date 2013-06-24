var vdb;
(function (vdb) {
    (function (viewModels) {
        var ArtistCreateViewModel = (function () {
            function ArtistCreateViewModel() {
                this.webLink = new viewModels.WebLinkEditViewModel();
            }
            return ArtistCreateViewModel;
        })();
        viewModels.ArtistCreateViewModel = ArtistCreateViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
