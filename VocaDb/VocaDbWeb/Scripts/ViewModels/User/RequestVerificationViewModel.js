var vdb;
(function (vdb) {
    (function (viewModels) {
        var RequestVerificationViewModel = (function () {
            function RequestVerificationViewModel(artistRepository) {
                var _this = this;
                this.artistRepository = artistRepository;
                this.clearArtist = function () {
                    _this.selectedArtist(null);
                };
                this.selectedArtist = ko.observable(null);
                this.setArtist = function (targetArtistId) {
                    _this.artistRepository.getOne(targetArtistId, function (artist) {
                        _this.selectedArtist(artist);
                    });
                };
                this.artistSearchParams = {
                    allowCreateNew: false,
                    acceptSelection: this.setArtist,
                    height: 300
                };
            }
            return RequestVerificationViewModel;
        })();
        viewModels.RequestVerificationViewModel = RequestVerificationViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
