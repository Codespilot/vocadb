var vdb;
(function (vdb) {
    (function (viewModels) {
        var PVRatingButtonsViewModel = (function () {
            function PVRatingButtonsViewModel(repository, songWithVoteContract, ratingCallback) {
                var _this = this;
                var songId = songWithVoteContract.Id;
                this.rating = ko.observable(songWithVoteContract.Vote);
                this.isRated = ko.computed(function () {
                    return _this.rating() != PVRatingButtonsViewModel.rating_nothing;
                });
                var setRating = function (rating) {
                    _this.rating(rating);
                    repository.updateSongRating(songId, rating, function () {
                        if(rating != PVRatingButtonsViewModel.rating_nothing && _.isFunction(ratingCallback)) {
                            ratingCallback();
                        }
                    });
                };
                this.setRating_favorite = function () {
                    return setRating(PVRatingButtonsViewModel.rating_favorite);
                };
                this.setRating_like = function () {
                    return setRating(PVRatingButtonsViewModel.rating_like);
                };
                this.setRating_nothing = function () {
                    return setRating(PVRatingButtonsViewModel.rating_nothing);
                };
            }
            PVRatingButtonsViewModel.rating_favorite = "Favorite";
            PVRatingButtonsViewModel.rating_like = "Like";
            PVRatingButtonsViewModel.rating_nothing = "Nothing";
            return PVRatingButtonsViewModel;
        })();
        viewModels.PVRatingButtonsViewModel = PVRatingButtonsViewModel;        
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
