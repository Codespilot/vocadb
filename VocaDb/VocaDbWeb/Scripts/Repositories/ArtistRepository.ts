/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

    // Repository for managing artists and related objects.
    // Corresponds to the ArtistController class.
    export class ArtistRepository {

        public findDuplicate: (params, callback: (result) => void ) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(baseUrl: string) {

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Artist") + relative;
            };

            this.findDuplicate = (params, callback: (result) => void ) => {

                $.post(this.mapUrl("/FindDuplicate"), params, callback);

            }

        }

    }

}