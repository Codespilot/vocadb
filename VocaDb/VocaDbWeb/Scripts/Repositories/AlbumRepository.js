var vdb;
(function (vdb) {
    (function (repositories) {
        var AlbumRepository = (function () {
            function AlbumRepository(baseUrl) {
                var _this = this;
                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
                };

                this.deleteArtistForAlbum = function (artistForAlbumId, callback) {
                    $.post(_this.mapUrl("/DeleteArtistForAlbum"), { artistForAlbumId: artistForAlbumId }, callback);
                };

                this.updateArtistForAlbumIsSupport = function (artistForAlbumId, isSupport, callback) {
                    $.post(_this.mapUrl("/UpdateArtistForAlbumIsSupport"), { artistForAlbumId: artistForAlbumId, isSupport: isSupport }, callback);
                };
            }
            return AlbumRepository;
        })();
        repositories.AlbumRepository = AlbumRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
