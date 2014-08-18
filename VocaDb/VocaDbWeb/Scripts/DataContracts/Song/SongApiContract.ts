
module vdb.dataContracts {
	
	export interface SongApiContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		artistString: string;

		id: number;

		localizedName: string;

		pvServices: string;

		ratingScore: number;

		thumbUrl: string;

	}

}