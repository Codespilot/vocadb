var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            

            var categories = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
            var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
            var data = { length: 39, webLinks: [webLinkData] };

            QUnit.module("SongEditViewModelTests");

            function createViewModel() {
                return new vm.SongEditViewModel(categories, data);
            }

            test("constructor", function () {
                var target = createViewModel();

                equal(target.length(), 39, "length");
                equal(target.lengthFormatted(), "0:39", "lengthFormatted");
                equal(target.webLinks.webLinks().length, 1, "webLinks.length");
            });

            test("lengthFormatted only seconds", function () {
                var target = createViewModel();

                target.lengthFormatted("39");

                equal(target.length(), 39, "length");
            });

            test("lengthFormatted over 1 minute", function () {
                var target = createViewModel();

                target.lengthFormatted("393");

                equal(target.length(), 393, "length");
            });

            test("lengthFormatted minutes and seconds", function () {
                var target = createViewModel();

                target.lengthFormatted("3:39");

                equal(target.length(), 219, "length");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
