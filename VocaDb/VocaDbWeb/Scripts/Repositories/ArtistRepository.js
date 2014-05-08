var vdb;
(function (vdb) {
    (function (repositories) {
        var ArtistRepository = (function () {
            function ArtistRepository(baseUrl) {
                var _this = this;
                this.baseUrl = baseUrl;
                this.getList = function (paging, lang, query, sort, artistTypes, tag, callback) {
                    var url = vdb.functions.mergeUrls(_this.baseUrl, "/api/artists");
                    var data = {
                        start: paging.start, getTotalCount: paging.getTotalCount, maxEntries: paging.maxEntries,
                        query: query, fields: "MainPicture", lang: lang, nameMatchMode: 'Auto', sort: sort,
                        artistTypes: artistTypes,
                        tag: tag
                    };

                    $.getJSON(url, data, callback);
                };
                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/Artist") + relative;
                };

                this.findDuplicate = function (params, callback) {
                    $.post(_this.mapUrl("/FindDuplicate"), params, callback);
                };

                this.getOne = function (id, callback) {
                    $.post(_this.mapUrl("/DataById"), { id: id }, callback);
                };
            }
            return ArtistRepository;
        })();
        repositories.ArtistRepository = ArtistRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
