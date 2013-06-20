var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var cls = vdb.models;
            var vm = vdb.viewModels;

            var repository = new vdb.tests.testSupport.FakeUserRepository();

            QUnit.module("PVRatingButtonsViewModel");

            test("constructor", function () {
                var target = new vm.PVRatingButtonsViewModel(repository, { Id: 39, Vote: cls.SongVoteRating[cls.SongVoteRating.Nothing] }, null);

                equal(cls.SongVoteRating.Nothing, target.rating(), "rating");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
