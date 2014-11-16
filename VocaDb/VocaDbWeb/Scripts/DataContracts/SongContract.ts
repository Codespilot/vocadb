/// <reference path="ArtistContract.ts" />

module vdb.dataContracts {

    export interface SongContract {

        additionalNames: string;

        artistString: string;

        id: number;

        name: string;

    }

    export interface SongWithComponentsContract extends SongContract {
        
        artists?: ArtistContract[];

		lengthSeconds: number;

		thumbUrl?: string;

        vote: string;

    }

}

