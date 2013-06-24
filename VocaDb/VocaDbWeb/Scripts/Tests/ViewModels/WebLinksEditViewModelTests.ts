/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/WebLinksEditViewModel.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;

    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };

    QUnit.module("WebLinksEditViewModel");

    test("constructor", () => {

        var target = new vm.WebLinksEditViewModel([webLinkData]);

        equal(target.webLinks().length, 1, "webLinks.length");

    });

    test("add new", () => {

        var target = new vm.WebLinksEditViewModel([]);

        target.add();

        equal(target.webLinks().length, 1, "webLinks.length");

    });

    test("remove", () => {

        var target = new vm.WebLinksEditViewModel([webLinkData]);

        target.remove(target.webLinks()[0]);

        equal(target.webLinks().length, 0, "webLinks.length");

    });

}