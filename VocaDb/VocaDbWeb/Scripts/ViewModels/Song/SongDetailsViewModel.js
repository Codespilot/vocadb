var vdb;
(function (vdb) {
    (function (viewModels) {
        var rep = vdb.repositories;

        var SongDetailsViewModel = (function () {
            function SongDetailsViewModel(repository, userRepository, data, ratingCallback) {
                var _this = this;
                this.id = data.id;
                this.userRating = new viewModels.PVRatingButtonsViewModel(userRepository, { id: data.id, vote: data.userRating }, ratingCallback);

                this.allVersionsVisible = ko.observable(false);

                this.getUsers = function () {
                    repository.usersWithSongRating(_this.id, function (result) {
                        _this.usersContent(result);
                        _this.usersPopupVisible(true);
                    });
                };

                this.showAllVersions = function () {
                    _this.allVersionsVisible(true);
                };

                this.usersContent = ko.observable();

                this.usersPopupVisible = ko.observable(false);
            }
            return SongDetailsViewModel;
        })();
        viewModels.SongDetailsViewModel = SongDetailsViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
