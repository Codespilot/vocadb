/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/SongEditViewModel.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;
	import dc = vdb.dataContracts;
	import sup = vdb.tests.testSupport;

    var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
    var data: vm.SongEdit = { artistLinks: [], length: 39, names: [], pvs: [], songType: 'Original', tags: [], webLinks: [webLinkData] }; 
	var artistRepo = new sup.FakeArtistRepository();
	var pvRepo = null;

    QUnit.module("SongEditViewModelTests");

    function createViewModel() {
		return new vm.SongEditViewModel(artistRepo, pvRepo, new vdb.UrlMapper(''), [], categories, data, false);
    }

    test("constructor", () => {

        var target = createViewModel();

        equal(target.length(), 39, "length");
        equal(target.lengthFormatted(), "0:39", "lengthFormatted");
        equal(target.webLinks.webLinks().length, 1, "webLinks.length");

    });

    test("lengthFormatted only seconds", () => {

        var target = createViewModel();

        target.lengthFormatted("39");
        
        equal(target.length(), 39, "length");

    });

    test("lengthFormatted over 1 minute", () => {

        var target = createViewModel();

        target.lengthFormatted("393");

        equal(target.length(), 393, "length");

    });

    test("lengthFormatted minutes and seconds", () => {

        var target = createViewModel();

        target.lengthFormatted("3:39");

        equal(target.length(), 219, "length");

    });

}