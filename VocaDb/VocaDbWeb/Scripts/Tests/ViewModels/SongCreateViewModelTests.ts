/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/SongCreateViewModel.ts" />
/// <reference path="../TestSupport/FakeSongRepository.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var repository = new vdb.tests.testSupport.FakeSongRepository();
    var producer: dc.ArtistContract = { id: 1, name: "Tripshots", additionalNames: "" };
    repository.results = { title: "Nebula", artists: [producer], matches: [], songType: "Original" };

    QUnit.module("SongCreateViewModelTests");

    function createViewModel() {
        return new vm.SongCreateViewModel(repository);
    }

    test("constructor empty", () => {

        var target = createViewModel();

        equal(target.nameOriginal(), "", "nameOriginal");
        equal(target.pv1(), "", "pv1");
        ok(target.artists(), "artists");
        equal(target.artists().length, 0, "artists.length");

    });

    test("constructor with data", () => {

        var target = new vm.SongCreateViewModel(repository, { nameOriginal: "Nebula", artists: [producer] });

        equal(target.nameOriginal(), "Nebula", "nameOriginal");
        ok(target.artists(), "artists");
        equal(target.artists().length, 1, "artists.length");

    });

    test("checkDuplicatesAndPV title and artists", () => {

        var target = createViewModel();

        target.checkDuplicatesAndPV();

        equal(target.nameOriginal(), "Nebula", "nameOriginal");
        ok(target.artists(), "artists");
        equal(target.artists().length, 1, "artists.length");

    });

    test("checkDuplicatesAndPV does not overwrite title", () => {

        var target = createViewModel();
        target.nameOriginal("Overridden title");

        target.checkDuplicatesAndPV();

        equal(target.nameOriginal(), "Overridden title", "nameOriginal");

    });

}
