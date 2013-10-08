var vdb;
(function (vdb) {
    (function (repositories) {
        var cls = vdb.models;
        var dc = vdb.dataContracts;

        var UserRepository = (function () {
            function UserRepository(urlMapper) {
                var _this = this;
                this.getMessageSummaries = function (maxCount, unread, iconSize, callback) {
                    if (typeof maxCount === "undefined") { maxCount = 200; }
                    if (typeof unread === "undefined") { unread = false; }
                    if (typeof iconSize === "undefined") { iconSize = 40; }
                    var url = _this.mapUrl("/MessagesJson");
                    $.getJSON(url, { maxCount: maxCount, unread: unread, iconSize: iconSize }, callback);
                };
                this.mapUrl = function (relative) {
                    return urlMapper.mapRelative("/User") + relative;
                };

                this.updateSongRating = function (songId, rating, callback) {
                    $.post(_this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: vdb.models.SongVoteRating[rating] }, callback);
                };
            }
            return UserRepository;
        })();
        repositories.UserRepository = UserRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
