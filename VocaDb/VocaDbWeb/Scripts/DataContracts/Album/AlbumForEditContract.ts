
module vdb.dataContracts.albums {
	
	export interface AlbumForEditContract {

		artistLinks: ArtistForAlbumContract[];

		defaultNameLanguage: string;

		description: string;

		discType: string;

		hasCover: boolean;

		identifiers: string[];

		names: globalization.LocalizedStringWithIdContract[];

		pictures: EntryPictureFileContract[];

		pvs: pvs.PVContract[];

		tracks: songs.SongInAlbumEditContract[];

		webLinks: WebLinkContract[];

	}

}