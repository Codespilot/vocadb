/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
var vdb;
(function (vdb) {
    (function (repositories) {
        // Repository for managing albums and related objects.
        // Corresponds to the AlbumController class.
        var AlbumRepository = (function () {
            function AlbumRepository(baseUrl) {
                var _this = this;
                this.baseUrl = baseUrl;
                this.getList = function (paging, lang, query, sort, discTypes, tag, artistId, artistParticipationStatus, status, callback) {
                    var url = vdb.functions.mergeUrls(_this.baseUrl, "/api/albums");
                    var data = {
                        start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
                        query: query, fields: "MainPicture", lang: lang, nameMatchMode: 'Auto', sort: sort,
                        discTypes: discTypes,
                        tag: tag,
                        artistId: artistId,
                        artistParticipationStatus: artistParticipationStatus,
                        status: status
                    };

                    $.getJSON(url, data, callback);
                };
                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
                };
            }
            return AlbumRepository;
        })();
        repositories.AlbumRepository = AlbumRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
