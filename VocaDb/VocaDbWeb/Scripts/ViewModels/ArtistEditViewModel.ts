/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

    export class ArtistEditViewModel {

        public submit = () => {
            this.submitting(true);
            return true;
        }

        public submitting = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

        constructor(webLinkCategories: vdb.dataContracts.TranslatedEnumField[], data: ArtistEdit) {

            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
        
        }

    }

    export interface ArtistEdit {

        webLinks: vdb.dataContracts.WebLinkContract[];

    }

}