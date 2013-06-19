var cls = vdb.models;

var vdb;
(function (vdb) {
    (function (repositories) {
        var UserRepository = (function () {
            function UserRepository(baseUrl) {
                var _this = this;
                this.mapUrl = function (relative) {
                    return baseUrl + "/User" + relative;
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
