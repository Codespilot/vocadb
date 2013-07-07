var vdb;
(function (vdb) {
    (function (viewModels) {
        var dc = vdb.dataContracts;
        var rep = vdb.repositories;

        var AlbumEditViewModel = (function () {
            function AlbumEditViewModel(repository, artistRoleNames, webLinkCategories, data) {
                var _this = this;
                this.repository = repository;
                this.artistLinks = ko.observableArray(_.map(data.artistLinks, function (artist) {
                    return new viewModels.ArtistForAlbumEditViewModel(repository, artist);
                }));

                this.getArtistLink = function (artistForAlbumId) {
                    return _.find(_this.artistLinks(), function (artist) {
                        return artist.id == artistForAlbumId;
                    });
                };

                this.removeArtist = function (artistForAlbum) {
                    _this.artistLinks.remove(artistForAlbum);
                    repository.deleteArtistForAlbum(artistForAlbum.id);
                };

                this.translateArtistRole = function (role) {
                    return artistRoleNames[role];
                };

                this.webLinks = new viewModels.WebLinksEditViewModel(data.webLinks, webLinkCategories);
            }
            return AlbumEditViewModel;
        })();
        viewModels.AlbumEditViewModel = AlbumEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
