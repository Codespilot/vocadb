function initEntrySearch() {
}

var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            var dc = vdb.dataContracts;

            var repository = new vdb.tests.testSupport.FakeSongRepository();
            var producer = { id: 1, name: "Tripshots", additionalNames: "" };
            repository.results = { title: "Nebula", artists: [producer], matches: [], songType: "Original" };

            QUnit.module("SongCreateViewModelTests");

            function createViewModel() {
                return new vm.SongCreateViewModel(repository);
            }

            test("constructor empty", function () {
                var target = createViewModel();

                equal(target.nameOriginal(), "", "nameOriginal");
                equal(target.pv1(), "", "pv1");
                ok(target.artists(), "artists");
                equal(target.artists().length, 0, "artists.length");
            });

            test("constructor with data", function () {
                var target = new vm.SongCreateViewModel(repository, { nameOriginal: "Nebula", artists: [producer] });

                equal(target.nameOriginal(), "Nebula", "nameOriginal");
                ok(target.artists(), "artists");
                equal(target.artists().length, 1, "artists.length");
            });

            test("checkDuplicatesAndPV title and artists", function () {
                var target = createViewModel();

                target.checkDuplicatesAndPV();

                equal(target.nameOriginal(), "Nebula", "nameOriginal");
                ok(target.artists(), "artists");
                equal(target.artists().length, 1, "artists.length");
            });

            test("checkDuplicatesAndPV does not overwrite title", function () {
                var target = createViewModel();
                target.nameOriginal("Overridden title");

                target.checkDuplicatesAndPV();

                equal(target.nameOriginal(), "Overridden title", "nameOriginal");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
