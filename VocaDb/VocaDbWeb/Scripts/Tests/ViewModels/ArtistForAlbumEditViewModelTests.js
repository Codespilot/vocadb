var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            

            var rep = new vdb.tests.testSupport.FakeAlbumRepository();
            var producer;
            var data;

            QUnit.module("ArtistForAlbumEditViewModelTests", {
                setup: function () {
                    producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
                    data = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
                }
            });

            function createViewModel() {
                return new vm.ArtistForAlbumEditViewModel(rep, data);
            }

            test("constructor", function () {
                var target = createViewModel();

                equal(target.isCustomizable(), true, "isCustomizable");
                equal(target.roles(), "Default", "roles");
                equal(target.rolesArray().length, 1, "rolesArray.length");
                equal(target.rolesArray()[0], "Default", "rolesArray[0]");
            });

            test("isCustomizable", function () {
                producer.artistType = "Vocaloid";
                var target = createViewModel();

                equal(target.isCustomizable(), false, "isCustomizable");
            });

            test("rolesArray write", function () {
                var target = createViewModel();

                target.rolesArray(['Composer', 'Arranger']);

                equal(target.roles(), "Composer,Arranger", "roles");
            });

            test("roles write", function () {
                var target = createViewModel();

                target.roles('Composer, Arranger');

                equal(target.rolesArray().length, 2, "rolesArray.length");
                equal(target.rolesArray()[0], "Composer", "rolesArray[0]");
                equal(target.rolesArray()[1], "Arranger", "rolesArray[1]");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
