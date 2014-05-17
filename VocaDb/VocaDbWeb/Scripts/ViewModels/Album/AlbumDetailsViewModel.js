/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

var vdb;
(function (vdb) {
    (function (viewModels) {
        var AlbumDetailsViewModel = (function () {
            function AlbumDetailsViewModel(id, formatString) {
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
                this.downloadTagsDialog = new DownloadTagsViewModel(id, formatString);
            }
            return AlbumDetailsViewModel;
        })();
        viewModels.AlbumDetailsViewModel = AlbumDetailsViewModel;

        var DownloadTagsViewModel = (function () {
            function DownloadTagsViewModel(albumId, formatString) {
                var _this = this;
                this.albumId = albumId;
                this.dialogVisible = ko.observable(false);
                this.downloadTags = function () {
                    _this.dialogVisible(false);

                    var url = "/Album/DownloadTags/" + _this.albumId;
                    window.location.href = url + "?setFormatString=true&formatString=" + encodeURIComponent(_this.formatString());
                };
                this.dialogButtons = ko.observableArray([
                    { text: vdb.resources.albumDetails.download, click: this.downloadTags }
                ]);
                this.show = function () {
                    _this.dialogVisible(true);
                };
                this.formatString = ko.observable(formatString);
            }
            return DownloadTagsViewModel;
        })();
        viewModels.DownloadTagsViewModel = DownloadTagsViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
