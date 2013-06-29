/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

    export class SongEditViewModel {

        public length: KnockoutObservable<number>;
        public lengthFormatted: KnockoutComputed<string>;
        public webLinks: WebLinksEditViewModel;

        private addLeadingZero(val) {
            return (val < 10 ? "0" + val : val);
        }

        constructor(webLinkCategories: vdb.dataContracts.TranslatedEnumField[], data: SongEdit) {

            this.length = ko.observable(data.length);
            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
            
            this.lengthFormatted = ko.computed({
                read: () => {
                    var mins = Math.floor(this.length() / 60);
                    return mins + ":" + this.addLeadingZero(this.length() % 60);
                },
                write: (value) => {
                    var parts = value.split(":");
                    if (parts.length == 2 && parseInt(parts[0], 10) != NaN && parseInt(parts[1], 10) != NaN) {
                        this.length(parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10));
                    } else if (parts.length == 1 && !isNaN(parseInt(parts[0], 10))) {
                        this.length(parseInt(parts[0], 10));
                    } else {
                        this.length(0);
                    }
                }
            });

        }

    }

    export interface SongEdit {

        length: number;

        webLinks: vdb.dataContracts.WebLinkContract[];

    }

}