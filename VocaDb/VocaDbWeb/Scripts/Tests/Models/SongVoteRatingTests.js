var vdb;
(function (vdb) {
    (function (tests) {
        (function (models) {
            var cls = vdb.models;

            QUnit.module("SongVoteRating");

            test("parseSongVoteRating nothing", function () {
                var result = cls.parseSongVoteRating("Nothing");

                equal(result, cls.SongVoteRating.Nothing, "result");
            });

            test("parseSongVoteRating like", function () {
                var result = cls.parseSongVoteRating("Like");

                equal(result, cls.SongVoteRating.Like, "result");
            });
        })(tests.models || (tests.models = {}));
        var models = tests.models;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
