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

        constructor(baseUrl: string) {

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Song") + relative;
            };

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {

                $.post(this.mapUrl("/FindDuplicate"), params, callback);

            }

            this.getOne = (id: number, includeArtists: boolean = false, callback?: (result: dc.SongWithComponentsContract) => void) => {

                $.post(this.mapUrl("/DataById"), { id: id, includeArtists: includeArtists }, callback);
            
            }

        }

    }

}