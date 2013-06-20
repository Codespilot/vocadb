var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var cls = vdb.models;
            var vm = vdb.viewModels;

            var repository = new vdb.tests.testSupport.FakeUserRepository();

            QUnit.module("PVRatingButtonsViewModel");

            function createTarget(songId, rating) {
                return new vm.PVRatingButtonsViewModel(repository, { Id: songId, Vote: cls.SongVoteRating[rating] }, null);
            }

            test("constructor", function () {
                var target = createTarget(39, cls.SongVoteRating.Nothing);

                equal(target.rating(), cls.SongVoteRating.Nothing, "rating");
                equal(target.isRated(), false, "isRated");
                equal(target.isRatingFavorite(), false, "isRatingFavorite");
                equal(target.isRatingLike(), false, "isRatingLike");
            });

            test("setRating_like", function () {
                var target = createTarget(39, cls.SongVoteRating.Nothing);
                target.setRating_like();

                equal(target.rating(), cls.SongVoteRating.Like, "rating");
                equal(target.isRated(), true, "isRated");
                equal(target.isRatingFavorite(), false, "isRatingFavorite");
                equal(target.isRatingLike(), true, "isRatingLike");
                equal(repository.songId, 39, "repository.songId");
                equal(repository.rating, cls.SongVoteRating.Like, "repository.rating");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
