/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Models/SongVoteRating.ts" />

import cls = vdb.models;

module vdb.repositories {

    // Repository for managing users and related objects.
    // Corresponds to the UserController class.
    export class UserRepository {

        // Updates rating score for a song.
        // songId: Id of the song to be updated.
        // rating: Song rating.
        // callback: Callback function to be executed when the operation is complete.
        public updateSongRating: (songId: number, rating: cls.SongVoteRating, callback: Function) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(baseUrl: string) {

            this.mapUrl = (relative: string) => {
                return baseUrl + "/User" + relative;
            };

            this.updateSongRating = (songId: number, rating: cls.SongVoteRating, callback: Function) => {

                $.post(this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: cls.SongVoteRating[rating] }, callback);

            }

        }

    }

}