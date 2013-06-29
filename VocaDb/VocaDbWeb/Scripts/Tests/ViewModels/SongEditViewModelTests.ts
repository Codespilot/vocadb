/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/SongEditViewModel.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
    var data: vm.SongEdit = { length: 39, webLinks: [webLinkData] }; 

    QUnit.module("SongEditViewModelTests");

    function createViewModel() {
        return new vm.SongEditViewModel(categories, data);
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