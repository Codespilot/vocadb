/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Repositories/ArtistRepository.ts" />
/// <reference path="WebLinkEditViewModel.ts" />

interface JQuery {
    vdbArtistToolTip: () => void;
}

module vdb.viewModels {

    export class ArtistCreateViewModel {
        
        public checkDuplicates: () => void;
        
        public nameOriginal = ko.observable("");
        public nameRomaji = ko.observable("");
        public nameEnglish = ko.observable("");

        public webLink: WebLinkEditViewModel = new WebLinkEditViewModel();

        constructor(artistRepository: vdb.repositories.ArtistRepository, data?) {
            
            if (data) {
                this.nameOriginal(data.nameOriginal || "");
                this.nameRomaji(data.nameRomaji || "");
                this.nameEnglish(data.nameEnglish || "");
            }

            this.checkDuplicates = () => {

                var term1 = this.nameOriginal();
                var term2 = this.nameRomaji();
                var term3 = this.nameEnglish();
                var linkUrl = this.webLink.url();

                // TODO: should be refactored to use knockout
                artistRepository.findDuplicate({ term1: term1, term2: term2, term3: term3, linkUrl: linkUrl }, result => {

                    if (result != "Ok") {
                        $("#duplicateEntryWarning").html(result);
                        $("#duplicateEntryWarning").show();
                        $("#duplicateEntryWarning a").vdbArtistToolTip();
                    } else {
                        $("#duplicateEntryWarning").hide();
                    }

                });

            }

        }
    
    }

}