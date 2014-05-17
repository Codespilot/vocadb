/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../dataContracts/ArtistContract.ts" />
/// <reference path="../dataContracts/ArtistForAlbumContract.ts" />
/// <reference path="../Repositories/AlbumRepository.ts" />
var vdb;
(function (vdb) {
    (function (viewModels) {
        // View model for editing artist for album link.
        var ArtistForAlbumEditViewModel = (function () {
            function ArtistForAlbumEditViewModel(repository, data) {
                var _this = this;
                this.artist = data.artist;
                this.id = data.id;
                this.isSupport = ko.observable(data.isSupport);

                this.name = data.name;
                this.rolesArray = ko.observableArray([]);

                this.isCustomizable = ko.computed(function () {
                    return !_this.artist || _.some(ArtistForAlbumEditViewModel.customizableArtistTypes, function (typeName) {
                        return _this.artist.artistType == typeName;
                    });
                });

                this.roles = ko.computed({
                    read: function () {
                        return _this.rolesArray().join();
                    },
                    write: function (value) {
                        _this.rolesArray(_.map(value.split(","), function (val) {
                            return val.trim();
                        }));
                    }
                });

                this.roles(data.roles);
            }
            ArtistForAlbumEditViewModel.customizableArtistTypes = [
                'Animator', 'OtherGroup', 'OtherIndividual',
                'OtherVocalist', 'Producer', 'Illustrator', 'Lyricist', 'Unknown', 'Utaite', 'Band'];
            return ArtistForAlbumEditViewModel;
        })();
        viewModels.ArtistForAlbumEditViewModel = ArtistForAlbumEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
