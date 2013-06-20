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
            var cls = vdb.models;

            var FakeUserRepository = (function (_super) {
                __extends(FakeUserRepository, _super);
                function FakeUserRepository() {
                    var _this = this;
                    _super.call(this, "");

                    this.updateSongRating = function (songId, rating, callback) {
                        _this.songId = songId;
                        _this.rating = rating;

                        if (callback)
                            callback();
                    };
                }
                return FakeUserRepository;
            })(vdb.repositories.UserRepository);
            testSupport.FakeUserRepository = FakeUserRepository;
        })(tests.testSupport || (tests.testSupport = {}));
        var testSupport = tests.testSupport;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
