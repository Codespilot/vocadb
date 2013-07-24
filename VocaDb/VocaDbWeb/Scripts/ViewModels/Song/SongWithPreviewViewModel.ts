/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../PVRatingButtonsViewModel.ts" />

module vdb.viewModels {

    import rep = vdb.repositories;

    // View model for song with PV preview and rating buttons (for example, on front page and song index page).
    export class SongWithPreviewViewModel {
        
        // Destroy PV player (clears HTML)
        public destroyPV: () => void;

        // Whether preview mode is active.
        public preview: KnockoutObservable<boolean> = ko.observable(false);

        // PV player HTML.
        public previewHtml: KnockoutObservable<string> = ko.observable(null);

        // View model for rating buttons.
        public ratingButtons: KnockoutObservable<PVRatingButtonsViewModel> = ko.observable(null);

        // Event handler for the event when the song has been rated.
        public ratingComplete: () => void;

        // Toggle preview status.
        public togglePreview: () => void;

        constructor(repository: rep.SongRepository, userRepository: rep.UserRepository, public songId: number) {
            
            this.destroyPV = () => {
                this.previewHtml(null);
            }

            this.togglePreview = () => {
                
                if (this.preview()) {
                    this.preview(false);
                    this.ratingButtons(null);
                    return;
                }

                repository.pvPlayerWithRating(songId, result => {

                    this.previewHtml(result.playerHtml);
                    var ratingButtonsViewModel = new PVRatingButtonsViewModel(userRepository, result.song, this.ratingComplete);
                    this.ratingButtons(ratingButtonsViewModel);
                    this.preview(true);

                });


            }

        }
    
    }

}