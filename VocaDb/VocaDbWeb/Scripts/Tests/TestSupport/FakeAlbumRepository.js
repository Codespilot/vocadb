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
            var FakeAlbumRepository = (function (_super) {
                __extends(FakeAlbumRepository, _super);
                function FakeAlbumRepository() {
                    var _this = this;
                    _super.call(this, "");

                    this.deleteArtistForAlbum = function (artistForAlbumId, callback) {
                        _this.deletedId = artistForAlbumId;
                        if (callback)
                            callback();
                    };

                    this.updateArtistForAlbumIsSupport = function (artistForAlbumId, isSupport, callback) {
                        _this.updatedId = artistForAlbumId;
                        if (callback)
                            callback();
                    };
                }
                return FakeAlbumRepository;
            })(vdb.repositories.AlbumRepository);
            testSupport.FakeAlbumRepository = FakeAlbumRepository;
        })(tests.testSupport || (tests.testSupport = {}));
        var testSupport = tests.testSupport;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
