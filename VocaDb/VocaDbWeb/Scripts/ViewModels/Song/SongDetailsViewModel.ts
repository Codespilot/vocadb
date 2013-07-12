/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../DataContracts/SongListBaseContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../PVRatingButtonsViewModel.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    // View model for the song details view.
    export class SongDetailsViewModel {
        
        public allVersionsVisible: KnockoutObservable<boolean>;

        public getUsers: () => void;

        public id: number;

        public showAllVersions: () => void;

        public songListDialog: SongListsViewModel;

        public usersContent: KnockoutObservable<string>;

        public usersPopupVisible: KnockoutObservable<boolean>;

        public userRating: PVRatingButtonsViewModel;

        constructor(repository: rep.SongRepository, userRepository: rep.UserRepository, resources: SongDetailsResources,
            data: SongDetailsAjax, ratingCallback: () => void ) {
            
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

            this.songListDialog = new SongListsViewModel(repository, resources, this.id);

            this.usersContent = ko.observable();

            this.usersPopupVisible = ko.observable(false);
        
        }
    
    }

    export class SongListsViewModel {
        
        public addedToList: () => void;

        public addSongToList: () => void;

        public dialogVisible: KnockoutObservable<boolean> = ko.observable(false);

        public newListName: KnockoutObservable<string> = ko.observable("");

        public selectedListId: KnockoutObservable<number> = ko.observable(null);

        public showSongLists: () => void;

        public songLists: KnockoutObservableArray<dc.SongListBaseContract> = ko.observableArray();

        constructor(repository: rep.SongRepository, resources: SongDetailsResources, songId: number) {
            
            var isValid = () => {
                return (this.selectedListId() != 0 || this.newListName().length > 0);
            };

            this.addSongToList = () => {
                if (isValid())
                    repository.addSongToList(this.selectedListId(), songId, this.newListName(), () => {
                        this.dialogVisible(false);
                        if (this.addedToList)
                            this.addedToList();
                    });
            }

            this.showSongLists = () => {
                repository.songListsForUser(songId, songLists => {

                    songLists.push({ id: 0, name: resources.createNewList })
                    this.songLists(songLists);
                    this.newListName("");
                    this.selectedListId(songLists.length > 0 ? songLists[0].id : undefined);
                    this.dialogVisible(true);

                });
            }
        
        }
    
    }

    export interface SongDetailsAjax {
        
        id: number;

        userRating: string;
    
    }

    export interface SongDetailsResources {

        createNewList: string;

    }

}
