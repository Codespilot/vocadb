/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

    // Repository for managing albums and related objects.
    // Corresponds to the AlbumController class.
    export class AlbumRepository {

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(baseUrl: string) {

            this.mapUrl = (relative) => {
                return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
            };

        }

    }

}