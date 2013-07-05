var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            var dc = vdb.dataContracts;

            var repository = new vdb.tests.testSupport.FakeSongRepository();
            var artistRepository = new vdb.tests.testSupport.FakeArtistRepository();
            var producer = { id: 1, name: "Tripshots", additionalNames: "" };
            artistRepository.result = producer;
            repository.results = { title: "Nebula", artists: [producer], matches: [], songType: "Original" };

            QUnit.module("SongCreateViewModelTests");

            function createViewModel() {
                return new vm.SongCreateViewModel(repository, artistRepository);
            }

            test("constructor empty", function () {
                var target = createViewModel();

                equal(target.nameOriginal(), "", "nameOriginal");
                equal(target.pv1(), "", "pv1");
                ok(target.artists(), "artists");
                equal(target.artists().length, 0, "artists.length");
            });

            test("constructor with data", function () {
                var target = new vm.SongCreateViewModel(repository, artistRepository, { nameOriginal: "Nebula", artists: [producer] });

                equal(target.nameOriginal(), "Nebula", "nameOriginal");
                ok(target.artists(), "artists");
                equal(target.artists().length, 1, "artists.length");
                equal(target.artists()[0].id, 1, "artist id");
            });

            test("addArtist", function () {
                var target = createViewModel();

                target.addArtist(1);

                equal(target.artists().length, 1, "artists.length");
                equal(target.artists()[0].id, 1, "artist id");
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
