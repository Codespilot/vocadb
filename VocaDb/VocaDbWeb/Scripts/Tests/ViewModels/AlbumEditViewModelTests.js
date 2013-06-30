var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            var dc = vdb.dataContracts;

            var categories = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
            var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
            var data = { webLinks: [webLinkData] };

            QUnit.module("AlbumEditViewModelTests");

            function createViewModel() {
                return new vm.AlbumEditViewModel(categories, data);
            }

            test("constructor", function () {
                var target = createViewModel();

                equal(target.webLinks.webLinks().length, 1, "webLinks.length");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
