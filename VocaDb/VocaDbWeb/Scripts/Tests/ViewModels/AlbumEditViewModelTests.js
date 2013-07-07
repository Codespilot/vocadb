var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            var dc = vdb.dataContracts;

            var rep = new vdb.tests.testSupport.FakeAlbumRepository();
            var categories = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
            var producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
            var artistLink = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
            var roles = { Default: "Default", VoiceManipulator: "Voice manipulator" };
            var webLinkData = { category: "Official", description: "Youtube Channel", id: 123, url: "http://www.youtube.com/user/tripshots" };
            var data = { artistLinks: [artistLink], webLinks: [webLinkData] };

            QUnit.module("AlbumEditViewModelTests");

            function createViewModel() {
                return new vm.AlbumEditViewModel(rep, roles, categories, data);
            }

            test("constructor", function () {
                var target = createViewModel();

                equal(target.artistLinks().length, 1, "artistLinks.length");
                equal(target.artistLinks()[0].id, 39, "artistLinks[0].id");
                equal(target.webLinks.webLinks().length, 1, "webLinks.length");
                equal(target.webLinks.webLinks()[0].id, 123, "webLinks[0].id");
            });

            test("getArtistLink found", function () {
                var target = createViewModel();

                var result = target.getArtistLink(39);

                ok(result, "result");
                equal(result.id, 39, "result.id");
            });

            test("getArtistLink not found", function () {
                var target = createViewModel();

                var result = target.getArtistLink(123);

                ok(!result, "result");
            });

            test("translateArtistRole", function () {
                var target = createViewModel();

                var result = target.translateArtistRole("VoiceManipulator");

                equal(result, "Voice manipulator", "result");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
