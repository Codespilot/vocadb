var vdb;
(function (vdb) {
    (function (viewModels) {
        var cls = vdb.models;

        var PVRatingButtonsViewModel = (function () {
            function PVRatingButtonsViewModel(repository, songWithVoteContract, ratingCallback) {
                var _this = this;
                var songId = songWithVoteContract.id;
                this.rating = ko.observable(cls.parseSongVoteRating(songWithVoteContract.vote));
                this.isRated = ko.computed(function () {
                    return _this.rating() != 0 /* Nothing */;
                });
                this.isRatingFavorite = ko.computed(function () {
                    return _this.rating() == 5 /* Favorite */;
                });
                this.isRatingLike = ko.computed(function () {
                    return _this.rating() == 3 /* Like */;
                });

                var setRating = function (rating) {
                    _this.rating(rating);
                    repository.updateSongRating(songId, rating, function () {
                        if (rating != 0 /* Nothing */ && _.isFunction(ratingCallback))
                            ratingCallback();
                    });
                };

                this.setRating_favorite = function () {
                    return setRating(5 /* Favorite */);
                };
                this.setRating_like = function () {
                    return setRating(3 /* Like */);
                };
                this.setRating_nothing = function () {
                    return setRating(0 /* Nothing */);
                };
            }
            return PVRatingButtonsViewModel;
        })();
        viewModels.PVRatingButtonsViewModel = PVRatingButtonsViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
