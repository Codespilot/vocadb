var vdb;
(function (vdb) {
    (function (repositories) {
        var AlbumRepository = (function () {
            function AlbumRepository(baseUrl) {
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
