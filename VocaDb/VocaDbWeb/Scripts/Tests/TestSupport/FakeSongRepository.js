/// <reference path="../../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
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
            var FakeSongRepository = (function (_super) {
                __extends(FakeSongRepository, _super);
                function FakeSongRepository() {
                    var _this = this;
                    _super.call(this, "");
                    this.results = null;
                    this.song = null;
                    this.songLists = [];

                    this.addSongToList = function (listId, songId, newListName, callback) {
                        _this.addedSongId = songId;

                        if (callback)
                            callback();
                    };

                    this.findDuplicate = function (params, callback) {
                        if (callback)
                            callback(_this.results);
                    };

                    this.getOne = function (id, includeArtists, callback) {
                        if (typeof includeArtists === "undefined") { includeArtists = false; }
                        if (callback)
                            callback(_this.song);
                    };

                    this.songListsForSong = function (songId, callback) {
                        if (callback)
                            callback("Miku!");
                    };

                    this.songListsForUser = function (ignoreSongId, callback) {
                        if (callback)
                            callback(_this.songLists);
                    };

                    this.usersWithSongRating = function (id, callback) {
                        if (callback)
                            callback("");
                    };
                }
                return FakeSongRepository;
            })(vdb.repositories.SongRepository);
            testSupport.FakeSongRepository = FakeSongRepository;
        })(tests.testSupport || (tests.testSupport = {}));
        var testSupport = tests.testSupport;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
