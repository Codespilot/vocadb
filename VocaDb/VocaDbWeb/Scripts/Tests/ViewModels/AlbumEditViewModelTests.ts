/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../TestSupport/FakeAlbumRepository.ts" />
/// <reference path="../../ViewModels/AlbumEditViewModel.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var rep = new vdb.tests.testSupport.FakeAlbumRepository();
    var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
    var producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
    var artistLink = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
    var roles = { Default: "Default", VoiceManipulator: "Voice manipulator" };
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 123, url: "http://www.youtube.com/user/tripshots" };
    var data: vm.AlbumEdit = { artistLinks: [artistLink], webLinks: [webLinkData] };

    QUnit.module("AlbumEditViewModelTests");

    function createViewModel() {
        return new vm.AlbumEditViewModel(rep, roles, categories, data);
    }

    test("constructor", () => {

        var target = createViewModel();

        equal(target.artistLinks().length, 1, "artistLinks.length");
        equal(target.artistLinks()[0].id, 39, "artistLinks[0].id");
        equal(target.webLinks.webLinks().length, 1, "webLinks.length");
        equal(target.webLinks.webLinks()[0].id, 123, "webLinks[0].id");

    });

    test("getArtistLink found", () => {

        var target = createViewModel();

        var result = target.getArtistLink(39);

        ok(result, "result");
        equal(result.id, 39, "result.id");

    });

    test("getArtistLink not found", () => {

        var target = createViewModel();

        var result = target.getArtistLink(123);

        ok(!result, "result");

    });

    test("translateArtistRole", () => {

        var target = createViewModel();

        var result = target.translateArtistRole("VoiceManipulator");

        equal(result, "Voice manipulator", "result");

    });

}