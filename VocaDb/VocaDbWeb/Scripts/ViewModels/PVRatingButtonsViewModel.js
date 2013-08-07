var vdb;
(function (vdb) {
    /// <reference path="../typings/knockout/knockout.d.ts" />
    /// <reference path="../typings/underscore/underscore.d.ts" />
    /// <reference path="../Repositories/UserRepository.ts" />
    /// <reference path="../Models/SongVoteRating.ts" />
    (function (viewModels) {
        var cls = vdb.models;

        // Knockout view model for PV rating buttons
        var PVRatingButtonsViewModel = (function () {
            function PVRatingButtonsViewModel(repository, songWithVoteContract, ratingCallback) {
                var _this = this;
                var songId = songWithVoteContract.id;
                this.rating = ko.observable(cls.parseSongVoteRating(songWithVoteContract.vote));
                this.isRated = ko.computed(function () {
                    return _this.rating() != cls.SongVoteRating.Nothing;
                });
                this.isRatingFavorite = ko.computed(function () {
                    return _this.rating() == cls.SongVoteRating.Favorite;
                });
                this.isRatingLike = ko.computed(function () {
                    return _this.rating() == cls.SongVoteRating.Like;
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
