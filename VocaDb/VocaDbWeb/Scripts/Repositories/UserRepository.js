/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Models/SongVoteRating.ts" />
var cls = vdb.models;

var vdb;
(function (vdb) {
    (function (repositories) {
        // Repository for managing users and related objects.
        // Corresponds to the UserController class.
        var UserRepository = (function () {
            function UserRepository(baseUrl) {
                var _this = this;
                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/User") + relative;
                };

                this.updateSongRating = function (songId, rating, callback) {
                    $.post(_this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: cls.SongVoteRating[rating] }, callback);
                };
            }
            return UserRepository;
        })();
        repositories.UserRepository = UserRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
