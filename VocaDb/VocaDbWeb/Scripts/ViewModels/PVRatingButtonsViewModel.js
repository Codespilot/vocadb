var vdb;
(function (vdb) {
    (function (viewModels) {
        var cls = vdb.models;

        var PVRatingButtonsViewModel = (function () {
            function PVRatingButtonsViewModel(repository, songWithVoteContract, ratingCallback) {
                var _this = this;
                var songId = songWithVoteContract.Id;
                this.rating = ko.observable(songWithVoteContract.Vote);
                this.isRated = ko.computed(function () {
                    return _this.rating() != cls.SongVoteRating.Nothing;
                });

                var setRating = function (rating) {
                    _this.rating(rating);
                    repository.updateSongRating(songId, rating, function () {
                        if (rating != cls.SongVoteRating.Nothing && _.isFunction(ratingCallback))
                            ratingCallback();
                    });
                };

                this.setRating_favorite = function () {
                    return setRating(cls.SongVoteRating.Favorite);
                };
                this.setRating_like = function () {
                    return setRating(cls.SongVoteRating.Like);
                };
                this.setRating_nothing = function () {
                    return setRating(cls.SongVoteRating.Nothing);
                };
            }
            return PVRatingButtonsViewModel;
        })();
        viewModels.PVRatingButtonsViewModel = PVRatingButtonsViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
