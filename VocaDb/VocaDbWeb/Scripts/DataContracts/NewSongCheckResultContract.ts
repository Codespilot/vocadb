/// <reference path="DuplicateEntryResultContract.ts" />

module vdb.dataContracts {

    export interface ArtistContract {

        additionalNames: string;

        id: number;

        name: string;

    }

    export interface NewSongCheckResultContract {

        artists: ArtistContract[];

        matches: DuplicateEntryResultContract[];

        songType: string;

        title: string;

    }

}