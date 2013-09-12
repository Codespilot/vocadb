var vdb;
(function (vdb) {
    (function (viewModels) {
        var AlbumDetailsViewModel = (function () {
            function AlbumDetailsViewModel(id) {
                var _this = this;
                this.id = id;
                this.usersContent = ko.observable();
                this.getUsers = function () {
                    $.post(vdb.functions.mapAbsoluteUrl("/Album/UsersWithAlbumInCollection"), { albumId: _this.id }, function (result) {
                        _this.usersContent(result);
                        $("#userCollectionsPopup").dialog("open");
                    });

                    return false;
                };
                this.downloadTagsDialog = new DownloadTagsViewModel(id);
            }
            return AlbumDetailsViewModel;
        })();
        viewModels.AlbumDetailsViewModel = AlbumDetailsViewModel;

        var DownloadTagsViewModel = (function () {
            function DownloadTagsViewModel(albumId) {
                var _this = this;
                this.albumId = albumId;
                this.dialogVisible = ko.observable(false);
                this.downloadOption = ko.observable(DownloadTagsViewModel.downloadOptionDefault);
                this.downloadOptionIsCustom = ko.computed(function () {
                    return _this.downloadOption() != DownloadTagsViewModel.downloadOptionDefault;
                });
                this.downloadTags = function () {
                    _this.dialogVisible(false);

                    var url = "/Album/DownloadTags/" + _this.albumId;
                    if (_this.downloadOptionIsCustom() && _this.formatString()) {
                        window.location.href = url + "?formatString=" + encodeURIComponent(_this.formatString());
                    } else {
                        window.location.href = url;
                    }
                };
                this.formatString = ko.observable("");
                this.dialogButtons = ko.observableArray([
                    { text: "Download", click: this.downloadTags }
                ]);
                this.show = function () {
                    _this.dialogVisible(true);
                };
            }
            DownloadTagsViewModel.downloadOptionDefault = "Default";
            return DownloadTagsViewModel;
        })();
        viewModels.DownloadTagsViewModel = DownloadTagsViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
