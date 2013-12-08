/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../dataContracts/ArtistContract.ts" />
/// <reference path="../dataContracts/ArtistForAlbumContract.ts" />
/// <reference path="../Repositories/AlbumRepository.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    // View model for editing artist for album link.
    export class ArtistForAlbumEditViewModel {
        
        private static customizableArtistTypes = ['Animator', 'OtherGroup', 'OtherIndividual',
            'OtherVocalist', 'Producer', 'Illustrator', 'Lyricist', 'Unknown'];

        public artist: dc.ArtistContract;

        // Unique link Id.
        public id: number;

        // Whether the roles of this artist can be customized.
        public isCustomizable: KnockoutComputed<boolean>;

        public isSupport: KnockoutObservable<boolean>;

        public name: string;

        // Roles as comma-separated string (for serializing to and from .NET enum for the server)
        public roles: KnockoutComputed<string>;

        // List of roles for this artist.
        public rolesArray: KnockoutObservableArray<string>;

        constructor(repository: rep.AlbumRepository, data: dc.ArtistForAlbumContract) {

            this.artist = data.artist;
            this.id = data.id;
            this.isSupport = ko.observable(data.isSupport);

            this.isSupport.subscribe(value => {
                repository.updateArtistForAlbumIsSupport(this.id, value);
            });

            this.name = data.name;
            this.rolesArray = ko.observableArray<string>([]);

            this.isCustomizable = ko.computed(() => {
                return !this.artist || _.some(ArtistForAlbumEditViewModel.customizableArtistTypes, typeName => this.artist.artistType == typeName);
            });

            this.roles = ko.computed({
                read: () => {
                    return this.rolesArray().join();
                },
                write: (value: string) => {
                    this.rolesArray(_.map(value.split(","), val => val.trim()));
                }
            });

            this.roles(data.roles);
        
        }
    
    }

}