/// <reference path="../typings/jquery/jquery.d.ts" />

module vdb.repositories {

    export class UserRepository {

        public updateSongRating: (songId: number, rating: string, callback: () => void) => void;

        private mapUrl: (relative: string) => string;

        constructor(baseUrl: string) {

            this.mapUrl = (relative: string) => {
                return baseUrl + "/User" + relative;
            };

            this.updateSongRating = (songId: number, rating: string, callback: () => void) => {

                $.post(this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: rating }, callback);

            }

        }

    }

}