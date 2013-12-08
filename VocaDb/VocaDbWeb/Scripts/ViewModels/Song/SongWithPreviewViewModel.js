var vdb;
(function (vdb) {
    (function (viewModels) {
        var rep = vdb.repositories;

        var SongWithPreviewViewModel = (function () {
            function SongWithPreviewViewModel(repository, userRepository, songId) {
                var _this = this;
                this.songId = songId;
                this.preview = ko.observable(false);
                this.previewHtml = ko.observable(null);
                this.ratingButtons = ko.observable(null);
                this.destroyPV = function () {
                    _this.previewHtml(null);
                };

                this.togglePreview = function () {
                    if (_this.preview()) {
                        _this.preview(false);
                        _this.ratingButtons(null);
                        return;
                    }

                    repository.pvPlayerWithRating(songId, function (result) {
                        _this.previewHtml(result.playerHtml);
                        var ratingButtonsViewModel = new vdb.viewModels.PVRatingButtonsViewModel(userRepository, result.song, _this.ratingComplete);
                        _this.ratingButtons(ratingButtonsViewModel);
                        _this.preview(true);
                    });
                };
            }
            return SongWithPreviewViewModel;
        })();
        viewModels.SongWithPreviewViewModel = SongWithPreviewViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
