/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="../Repositories/AlbumRepository.ts" />
/// <reference path="ArtistForAlbumEditViewModel.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    export class AlbumEditViewModel {
        
        public artistLinks: KnockoutObservableArray<ArtistForAlbumEditViewModel>;

        public getArtistLink: (artistForAlbumId: number) => ArtistForAlbumEditViewModel;

        public removeArtist: (artist: ArtistForAlbumEditViewModel) => void;

        public translateArtistRole: (role: string) => string;

        public webLinks: WebLinksEditViewModel;
        
        constructor(public repository: rep.AlbumRepository, artistRoleNames, webLinkCategories: dc.TranslatedEnumField[], data: AlbumEdit) {

            this.artistLinks = ko.observableArray(_.map(data.artistLinks, artist => new ArtistForAlbumEditViewModel(repository, artist)));

            this.getArtistLink = (artistForAlbumId) => {
                return _.find(this.artistLinks(), artist => artist.id == artistForAlbumId);
            };

            this.removeArtist = artistForAlbum => {
                this.artistLinks.remove(artistForAlbum);
                repository.deleteArtistForAlbum(artistForAlbum.id);
            };

            this.translateArtistRole = (role) => {
                return artistRoleNames[role];
            };

            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
            
        }

    }

    export interface AlbumEdit {
        
        artistLinks: dc.ArtistForAlbumContract[];

        webLinks: dc.WebLinkContract[];
    
    }

}