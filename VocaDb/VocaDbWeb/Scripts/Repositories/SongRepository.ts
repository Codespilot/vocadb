/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../DataContracts/SongContract.ts" />
/// <reference path="../DataContracts/SongListBaseContract.ts" />
/// <reference path="../DataContracts/Song/SongListContract.ts" />
/// <reference path="../DataContracts/Song/SongWithPVPlayerAndVoteContract.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

    import dc = vdb.dataContracts;

    // Repository for managing songs and related objects.
    // Corresponds to the SongController class.
    export class SongRepository {

        public addSongToList: (listId: number, songId: number, newListName: string, callback?: Function) => void;

        public findDuplicate: (params, callback: (result: dc.NewSongCheckResultContract) => void) => void;

        private get: (relative: string, params: any, callback: Function) => void;

        public getOne: (id: number, includeArtists: boolean, callback?: (result: dc.SongWithComponentsContract) => void) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        private post: (relative: string, params: any, callback: Function) => void;

        public pvPlayerWithRating: (songId: number, callback: (result: dc.SongWithPVPlayerAndVoteContract) => void) => void; 

        public songListsForSong: (songId: number, callback: (result: dc.SongListContract[]) => void) => void;

        public songListsForUser: (ignoreSongId: number, callback: (result: dc.SongListBaseContract[]) => void ) => void;

        public usersWithSongRating: (id: number, callback: (result: string) => void) => void;

        constructor(baseUrl: string) {

            this.get = (relative, params, callback) => {
                $.getJSON(this.mapUrl(relative), params, callback);
            }

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Song") + relative;
            };

            this.post = (relative, params, callback) => {
                $.post(this.mapUrl(relative), params, callback);
            }

            this.addSongToList = (listId, songId, newListName, callback?) => {
                this.post("/AddSongToList", { listId: listId, songId: songId, newListName: newListName }, callback);
            }

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {
                this.post("/FindDuplicate", params, callback);
            }

            this.getOne = (id: number, includeArtists: boolean = false, callback?: (result: dc.SongWithComponentsContract) => void) => {
                this.post("/DataById", { id: id, includeArtists: includeArtists }, callback);         
            }

            this.pvPlayerWithRating = (songId, callback) => {
                this.get("/PVPlayerWithRating", { songId: songId }, callback);
            }

            this.songListsForUser = (ignoreSongId, callback) => {                
                this.post("/SongListsForUser", { ignoreSongId: ignoreSongId }, callback);
            }

            this.usersWithSongRating = (id, callback: (result: string) => void ) => {
                this.post("/UsersWithSongRating", { songId: id }, callback);
            }

        }

    }

}