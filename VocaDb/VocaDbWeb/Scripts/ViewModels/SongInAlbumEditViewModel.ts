/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../dataContracts/ArtistContract.ts" />
/// <reference path="../dataContracts/SongContract.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;

    export class SongInAlbumEditViewModel {
        
        public artists: KnockoutObservableArray<dc.ArtistContract>;

        public artistString: KnockoutObservable<string>;

        public discNumber: KnockoutObservable<number>;

        public songAdditionalNames: string;

        public songId: number;
        
        public songInAlbumId: number;

        public songName: string;

        public trackNumber: KnockoutObservable<number>;

        constructor(data: SongInAlbumEditContract) {
            
            this.artists = ko.observableArray(data.artists);
            this.artistString = ko.observable(data.artistString);
            this.discNumber = ko.observable(data.discNumber);
            this.songAdditionalNames = data.songAdditionalNames;
            this.songId = data.songId;
            this.songInAlbumId = data.songInAlbumId;
            this.songName = data.songName;
            this.trackNumber = ko.observable(data.trackNumber);

            // TODO
            this.artists.subscribe(() => {
                this.artistString(this.artists().join(","));
            });
        
        }

    }

    export interface SongInAlbumEditContract {

        artists: dc.ArtistContract[];

        artistString: string;

        discNumber: number;

        songAdditionalNames: string;

        songId: number;
        
        songInAlbumId: number;

        songName: string;

        trackNumber: number;


    }

}