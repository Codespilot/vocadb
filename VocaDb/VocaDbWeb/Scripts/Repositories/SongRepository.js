var vdb;
(function (vdb) {
    (function (repositories) {
        var dc = vdb.dataContracts;

        var SongRepository = (function () {
            function SongRepository(baseUrl) {
                var _this = this;
                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/Song") + relative;
                };

                this.findDuplicate = function (params, callback) {
                    $.post(_this.mapUrl("/FindDuplicate"), params, callback);
                };

                this.getOne = function (id, includeArtists, callback) {
                    if (typeof includeArtists === "undefined") { includeArtists = false; }
                    $.post(_this.mapUrl("/DataById"), { id: id, includeArtists: includeArtists }, callback);
                };
            }
            return SongRepository;
        })();
        repositories.SongRepository = SongRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
