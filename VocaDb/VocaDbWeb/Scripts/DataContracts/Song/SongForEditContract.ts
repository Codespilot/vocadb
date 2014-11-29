
module vdb.dataContracts.songs {
	
	export interface SongForEditContract {

		artistLinks: ArtistForAlbumContract[];

		length: number;

		names: globalization.LocalizedStringWithIdContract[];

		pvs: pvs.PVContract[];

		songType: string;

		tags: string[];

		webLinks: vdb.dataContracts.WebLinkContract[];

	}

}