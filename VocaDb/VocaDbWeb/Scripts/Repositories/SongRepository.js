/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../DataContracts/SongContract.ts" />
/// <reference path="../DataContracts/SongListBaseContract.ts" />
/// <reference path="../DataContracts/Song/SongListContract.ts" />
/// <reference path="../DataContracts/Song/SongWithPVPlayerAndVoteContract.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
var vdb;
(function (vdb) {
    (function (repositories) {
        // Repository for managing songs and related objects.
        // Corresponds to the SongController class.
        var SongRepository = (function () {
            function SongRepository(baseUrl) {
                var _this = this;
                this.baseUrl = baseUrl;
                this.getList = function (paging, lang, query, sort, songTypes, tag, artistId, onlyWithPvs, status, callback) {
                    var url = vdb.functions.mergeUrls(_this.baseUrl, "/api/songs");
                    var data = {
                        start: paging.start, getTotalCount: paging.getTotalCount, maxEntries: paging.maxEntries,
                        query: query, fields: "ThumbUrl", lang: lang, nameMatchMode: 'Auto', sort: sort,
                        songTypes: songTypes,
                        tag: tag,
                        artistId: artistId,
                        onlyWithPvs: onlyWithPvs,
                        status: status
                    };

                    $.getJSON(url, data, callback);
                };
                this.get = function (relative, params, callback) {
                    $.get(_this.mapUrl(relative), params, callback);
                };

                this.getJSON = function (relative, params, callback) {
                    $.getJSON(_this.mapUrl(relative), params, callback);
                };

                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/Song") + relative;
                };

                this.post = function (relative, params, callback) {
                    $.post(_this.mapUrl(relative), params, callback);
                };

                this.addSongToList = function (listId, songId, newListName, callback) {
                    _this.post("/AddSongToList", { listId: listId, songId: songId, newListName: newListName }, callback);
                };

                this.findDuplicate = function (params, callback) {
                    _this.post("/FindDuplicate", params, callback);
                };

                this.getOne = function (id, includeArtists, callback) {
                    if (typeof includeArtists === "undefined") { includeArtists = false; }
                    _this.post("/DataById", { id: id, includeArtists: includeArtists }, callback);
                };

                this.pvPlayerWithRating = function (songId, callback) {
                    _this.getJSON("/PVPlayerWithRating", { songId: songId }, callback);
                };

                this.songListsForSong = function (songId, callback) {
                    _this.get("/SongListsForSong", { songId: songId }, callback);
                };

                this.songListsForUser = function (ignoreSongId, callback) {
                    _this.post("/SongListsForUser", { ignoreSongId: ignoreSongId }, callback);
                };

                this.usersWithSongRating = function (id, callback) {
                    _this.post("/UsersWithSongRating", { songId: id }, callback);
                };
            }
            return SongRepository;
        })();
        repositories.SongRepository = SongRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
