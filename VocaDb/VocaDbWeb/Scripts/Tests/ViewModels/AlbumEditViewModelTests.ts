/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/AlbumEditViewModel.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
    var data: vm.AlbumEdit = { webLinks: [webLinkData] };

    QUnit.module("AlbumEditViewModelTests");

    function createViewModel() {
        return new vm.AlbumEditViewModel(categories, data);
    }

    test("constructor", () => {

        var target = createViewModel();

        equal(target.webLinks.webLinks().length, 1, "webLinks.length");

    });

}