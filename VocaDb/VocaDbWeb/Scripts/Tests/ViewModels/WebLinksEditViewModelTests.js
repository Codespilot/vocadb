var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var dc = vdb.dataContracts;
            var vm = vdb.viewModels;

            var categories = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
            var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };

            QUnit.module("WebLinksEditViewModel");

            test("constructor", function () {
                var target = new vm.WebLinksEditViewModel([webLinkData], categories);

                equal(target.webLinks().length, 1, "webLinks.length");
                equal(target.categories.length, 2, "categories.length");
            });

            test("add new", function () {
                var target = new vm.WebLinksEditViewModel([]);

                target.add();

                equal(target.webLinks().length, 1, "webLinks.length");
            });

            test("remove", function () {
                var target = new vm.WebLinksEditViewModel([webLinkData]);

                target.remove(target.webLinks()[0]);

                equal(target.webLinks().length, 0, "webLinks.length");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
