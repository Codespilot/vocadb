/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

    export class AlbumEditViewModel {
        
        public webLinks: WebLinksEditViewModel;
        
        constructor(webLinkCategories: vdb.dataContracts.TranslatedEnumField[], data: AlbumEdit) {

            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
            
        }

    }

    export interface AlbumEdit {
        
        webLinks: vdb.dataContracts.WebLinkContract[];
    
    }

}