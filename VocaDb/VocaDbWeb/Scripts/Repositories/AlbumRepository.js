var vdb;
(function (vdb) {
    (function (repositories) {
        var AlbumRepository = (function () {
            function AlbumRepository(baseUrl) {
                var _this = this;
                this.baseUrl = baseUrl;
                this.getList = function (paging, lang, query, sort, discTypes, tag, artistId, artistParticipationStatus, callback) {
                    var url = vdb.functions.mergeUrls(_this.baseUrl, "/api/albums");
                    var data = {
                        start: paging.start, getTotalCount: paging.getTotalCount, maxEntries: paging.maxEntries,
                        query: query, fields: "MainPicture", lang: lang, nameMatchMode: 'Auto', sort: sort,
                        discTypes: discTypes,
                        tag: tag,
                        artistId: artistId,
                        artistParticipationStatus: artistParticipationStatus
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
