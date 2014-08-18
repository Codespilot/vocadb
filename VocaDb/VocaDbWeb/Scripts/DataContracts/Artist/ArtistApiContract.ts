
module vdb.dataContracts {

	export interface ArtistApiContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		id: number;

		localizedName: string;

		mainPicture: EntryThumbContract;

	}

} 