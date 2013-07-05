/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../dataContracts/ArtistContract.ts" />

module vdb.viewModels {

    export class ArtistForAlbumEditViewModel {
        
        public artist: KnockoutObservable<vdb.dataContracts.ArtistContract>;

        public id: number;

        public isSupport: KnockoutObservable<boolean>;

        public name: string;

        public roles: string;
    
    }

}