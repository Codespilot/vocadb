/// <reference path="../../Repositories/AlbumRepository.ts" />
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
                    _super.call(this, "");
                }
                return FakeAlbumRepository;
            })(vdb.repositories.AlbumRepository);
            testSupport.FakeAlbumRepository = FakeAlbumRepository;
        })(tests.testSupport || (tests.testSupport = {}));
        var testSupport = tests.testSupport;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
