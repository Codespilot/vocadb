/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../PVRatingButtonsViewModel.ts" />

module vdb.viewModels {

    import rep = vdb.repositories;

    // View model for the song details view.
    export class SongDetailsViewModel {
        
        public allVersionsVisible: KnockoutObservable<boolean>;

        public getUsers: () => void;

        public id: number;

        public showAllVersions: () => void;

        public usersContent: KnockoutObservable<string>;

        public usersPopupVisible: KnockoutObservable<boolean>;

        public userRating: PVRatingButtonsViewModel;

        constructor(repository: rep.SongRepository, userRepository: rep.UserRepository, data: SongDetailsAjax, ratingCallback: () => void) {
            
            this.id = data.id;
            this.userRating = new PVRatingButtonsViewModel(userRepository, { id: data.id, vote: data.userRating }, ratingCallback);

            this.allVersionsVisible = ko.observable(false);

            this.getUsers = () => {
                repository.usersWithSongRating(this.id, result => {
                    this.usersContent(result);
                    this.usersPopupVisible(true);
                });
            };

            this.showAllVersions = () => {
                this.allVersionsVisible(true);
            };

            this.usersContent = ko.observable();

            this.usersPopupVisible = ko.observable(false);
        
        }
    
    }

    export interface SongDetailsAjax {
        
        id: number;

        userRating: string;
    
    }

}