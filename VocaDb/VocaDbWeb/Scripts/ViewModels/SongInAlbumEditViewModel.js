var vdb;
(function (vdb) {
    (function (viewModels) {
        var dc = vdb.dataContracts;

        var SongInAlbumEditViewModel = (function () {
            function SongInAlbumEditViewModel(data) {
                var _this = this;
                this.artists = ko.observableArray(data.artists);
                this.artistString = ko.observable(data.artistString);
                this.discNumber = ko.observable(data.discNumber);
                this.songAdditionalNames = data.songAdditionalNames;
                this.songId = data.songId;
                this.songInAlbumId = data.songInAlbumId;
                this.songName = data.songName;
                this.trackNumber = ko.observable(data.trackNumber);

                this.isNextDisc = ko.observable(this.trackNumber() == 1 && this.discNumber() > 1);
                this.selected = ko.observable(false);

                this.artists.subscribe(function () {
                    _this.artistString(_.map(_this.artists(), function (a) {
                        return a.name;
                    }).join(", "));
                });
            }
            return SongInAlbumEditViewModel;
        })();
        viewModels.SongInAlbumEditViewModel = SongInAlbumEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
