/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../DataContracts/SongContract.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

    import dc = vdb.dataContracts;

    // Repository for managing songs and related objects.
    // Corresponds to the SongController class.
    export class SongRepository {

        public findDuplicate: (params, callback: (result: dc.NewSongCheckResultContract) => void) => void;

        public getOne: (id: number, includeArtists: boolean, callback?: (result: dc.SongWithComponentsContract) => void) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        private post: (relative: string, params: any, callback: Function) => void;

        public usersWithSongRating: (id: number, callback: (result: string) => void) => void;

        constructor(baseUrl: string) {

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Song") + relative;
            };

            this.post = (relative, params, callback) => {
                $.post(this.mapUrl(relative), params, callback);
            }

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {

                this.post("/FindDuplicate", params, callback);

            }

            this.getOne = (id: number, includeArtists: boolean = false, callback?: (result: dc.SongWithComponentsContract) => void) => {

                this.post("/DataById", { id: id, includeArtists: includeArtists }, callback);
            
            }

            this.usersWithSongRating = (id, callback: (result: string) => void ) => {

                this.post("/UsersWithSongRating", { songId: id }, callback);

            }

        }

    }

}