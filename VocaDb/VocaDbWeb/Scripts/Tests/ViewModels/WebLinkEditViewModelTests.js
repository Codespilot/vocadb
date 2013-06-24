var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;

            var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };

            QUnit.module("WebLinkEditViewModel");

            test("constructor", function () {
                var target = new vm.WebLinkEditViewModel(webLinkData);

                equal(target.category(), "Official", "category");
                equal(target.description(), "Youtube Channel", "description");
                equal(target.url(), "http://www.youtube.com/user/tripshots", "url");
            });

            test("editing url sets description", function () {
                var target = new vm.WebLinkEditViewModel(null);

                target.url("http://www.nicovideo.jp/mylist/");

                equal(target.category(), "Official", "category");
                equal(target.description(), "NND MyList", "description");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
