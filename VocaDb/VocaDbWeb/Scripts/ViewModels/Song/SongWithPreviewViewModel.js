/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../PVRatingButtonsViewModel.ts" />
var vdb;
(function (vdb) {
    (function (viewModels) {
        // View model for song with PV preview and rating buttons (for example, on front page and song index page).
        var SongWithPreviewViewModel = (function () {
            function SongWithPreviewViewModel(repository, userRepository, songId) {
                var _this = this;
                this.songId = songId;
                // Whether preview mode is active.
                this.preview = ko.observable(false);
                // PV player HTML.
                this.previewHtml = ko.observable(null);
                // View model for rating buttons.
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
                        var ratingButtonsViewModel = new viewModels.PVRatingButtonsViewModel(userRepository, result.song, _this.ratingComplete);
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
