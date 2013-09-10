/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

module vdb.viewModels {

    export class AlbumDetailsViewModel {

        public downloadTagsDialog: DownloadTagsViewModel;

        public usersContent: KnockoutObservable<string> = ko.observable();

        public getUsers = () => {

            $.post(vdb.functions.mapAbsoluteUrl("/Album/UsersWithAlbumInCollection"), { albumId: this.id }, result => {

                this.usersContent(result);
                $("#userCollectionsPopup").dialog("open");

            });

            return false;

        };

        constructor(private id: number) {
            this.downloadTagsDialog = new DownloadTagsViewModel(id);
        }

    }

    export class DownloadTagsViewModel {

        private static downloadOptionDefault = "Default";

        public dialogVisible = ko.observable(false);

        public downloadOption = ko.observable(DownloadTagsViewModel.downloadOptionDefault);

        public downloadOptionIsCustom = ko.computed(() =>
            this.downloadOption() != DownloadTagsViewModel.downloadOptionDefault
        );

        public downloadTags = () => {

            this.dialogVisible(false);

            var url = "/Album/DownloadTags/" + this.albumId;
            if (this.downloadOptionIsCustom() && this.formatString()) {
                window.location.href = url + "?formatString=" + this.formatString();
            } else {
                window.location.href = url;
            }

        };

        public formatString = ko.observable("");

        public dialogButtons = ko.observableArray([
            { text: "Download", click: this.downloadTags },
        ]);

        public show = () => {

            this.dialogVisible(true);

        };

        constructor(private albumId: number) { }

    }

}