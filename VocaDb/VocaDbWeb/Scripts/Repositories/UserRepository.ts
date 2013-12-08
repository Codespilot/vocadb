/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/User/UserMessageSummaryContract.ts" />
/// <reference path="../DataContracts/User/UserMessagesContract.ts" />
/// <reference path="../Models/SongVoteRating.ts" />

module vdb.repositories {

    import cls = vdb.models;
    import dc = vdb.dataContracts;

    // Repository for managing users and related objects.
    // Corresponds to the UserController class.
    export class UserRepository {

        public getMessageBody = (messageId: number, callback?: (result: string) => void) => {

            var url = this.mapUrl("/MessageBody");
            $.get(url, { messageId: messageId }, callback);

        };

        public getMessageSummaries = (maxCount: number = 200, unread: boolean = false, iconSize: number = 40, callback?: (result: dc.UserMessagesContract) => void ) => {

            var url = this.mapUrl("/MessagesJson");
            $.getJSON(url, { maxCount: maxCount, unread: unread, iconSize: iconSize }, callback);

        };

        // Updates rating score for a song.
        // songId: Id of the song to be updated.
        // rating: Song rating.
        // callback: Callback function to be executed when the operation is complete.
        public updateSongRating: (songId: number, rating: vdb.models.SongVoteRating, callback: any) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(urlMapper: vdb.UrlMapper) {

            this.mapUrl = (relative: string) => {
                return urlMapper.mapRelative("/User") + relative;
            };

            this.updateSongRating = (songId: number, rating: cls.SongVoteRating, callback: any) => {

                $.post(this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: vdb.models.SongVoteRating[rating] }, callback);

            }

        }

    }

}