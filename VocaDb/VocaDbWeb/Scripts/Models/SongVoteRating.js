var vdb;
(function (vdb) {
    (function (models) {
        (function (SongVoteRating) {
            SongVoteRating[SongVoteRating["Nothing"] = 0] = "Nothing";
            SongVoteRating[SongVoteRating["Like"] = 3] = "Like";
            SongVoteRating[SongVoteRating["Favorite"] = 5] = "Favorite";
        })(models.SongVoteRating || (models.SongVoteRating = {}));
        var SongVoteRating = models.SongVoteRating;

        function parseSongVoteRating(rating) {
            switch (rating) {
                case "Nothing":
                    return SongVoteRating.Nothing;
                case "Like":
                    return SongVoteRating.Like;
                case "Favorite":
                    return SongVoteRating.Favorite;
            }
        }
        models.parseSongVoteRating = parseSongVoteRating;
    })(vdb.models || (vdb.models = {}));
    var models = vdb.models;
})(vdb || (vdb = {}));
