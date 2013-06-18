/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../Repositories/UserRepository.ts" />

module vdb.viewModels {

    export class PVRatingButtonsViewModel {

        public static rating_favorite = "Favorite";
        public static rating_like = "Like";
        public static rating_nothing = "Nothing";

        public isRated: KnockoutComputed;

        public rating: KnockoutObservableAny;

        public setRating_favorite: () => void;

        public setRating_like: () => void;

        public setRating_nothing: () => void;

        constructor(repository: vdb.repositories.UserRepository, songWithVoteContract, ratingCallback: () => void) {

            var songId = songWithVoteContract.Id;
            this.rating = ko.observable(songWithVoteContract.Vote);
            this.isRated = ko.computed(() => this.rating() != PVRatingButtonsViewModel.rating_nothing);

            var setRating = (rating: string) => {
                this.rating(rating);
                repository.updateSongRating(songId, rating, () => {
                    if (rating != PVRatingButtonsViewModel.rating_nothing &&  _.isFunction(ratingCallback))
                        ratingCallback();
                });
            }

            this.setRating_favorite = () => setRating(PVRatingButtonsViewModel.rating_favorite);
            this.setRating_like = () => setRating(PVRatingButtonsViewModel.rating_like);
            this.setRating_nothing = () => setRating(PVRatingButtonsViewModel.rating_nothing);

        }

    }

}