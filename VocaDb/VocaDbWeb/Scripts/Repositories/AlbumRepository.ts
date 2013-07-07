/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

    // Repository for managing albums and related objects.
    // Corresponds to the AlbumController class.
    export class AlbumRepository {

        // Deletes an artist for album by link Id.
        public deleteArtistForAlbum: (artistForAlbumId: number, callback?: () => void) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        public updateArtistForAlbumIsSupport: (artistForAlbumId: number, isSupport: boolean, callback?: () => void ) => void;

        constructor(baseUrl: string) {

            this.mapUrl = (relative) => {
                return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
            };

            this.deleteArtistForAlbum = (artistForAlbumId, callback?) => {

                $.post(this.mapUrl("/DeleteArtistForAlbum"), { artistForAlbumId: artistForAlbumId }, callback);
            
            }

            this.updateArtistForAlbumIsSupport = (artistForAlbumId, isSupport, callback?) => {
                
                $.post(this.mapUrl("/UpdateArtistForAlbumIsSupport"), { artistForAlbumId: artistForAlbumId, isSupport: isSupport }, callback);

            }

        }

    }

}