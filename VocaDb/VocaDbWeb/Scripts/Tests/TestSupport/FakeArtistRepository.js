var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var vdb;
(function (vdb) {
    (function (tests) {
        (function (testSupport) {
            var dc = vdb.dataContracts;

            var FakeArtistRepository = (function (_super) {
                __extends(FakeArtistRepository, _super);
                function FakeArtistRepository() {
                    var _this = this;
                    _super.call(this, "");
                    this.result = null;

                    this.getOne = function (id, callback) {
                        if (callback)
                            callback(_this.result);
                    };
                }
                return FakeArtistRepository;
            })(vdb.repositories.ArtistRepository);
            testSupport.FakeArtistRepository = FakeArtistRepository;
        })(tests.testSupport || (tests.testSupport = {}));
        var testSupport = tests.testSupport;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
