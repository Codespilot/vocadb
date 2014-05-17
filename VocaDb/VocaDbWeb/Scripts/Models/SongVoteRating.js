var vdb;
(function (vdb) {
    (function (models) {
        // Song vote values.
        // Corresponds to the enum SongVoteRating.
        (function (SongVoteRating) {
            SongVoteRating[SongVoteRating["Nothing"] = 0] = "Nothing";
            SongVoteRating[SongVoteRating["Like"] = 3] = "Like";
            SongVoteRating[SongVoteRating["Favorite"] = 5] = "Favorite";
        })(models.SongVoteRating || (models.SongVoteRating = {}));
        var SongVoteRating = models.SongVoteRating;

        function parseSongVoteRating(rating) {
            switch (rating) {
                case "Nothing":
                    return 0 /* Nothing */;
                case "Like":
                    return 3 /* Like */;
                case "Favorite":
                    return 5 /* Favorite */;
            }
        }
        models.parseSongVoteRating = parseSongVoteRating;
    })(vdb.models || (vdb.models = {}));
    var models = vdb.models;
})(vdb || (vdb = {}));
