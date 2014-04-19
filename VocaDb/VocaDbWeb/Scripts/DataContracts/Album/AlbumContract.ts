
module vdb.dataContracts {
	
	export interface AlbumContract {

		additionalNames: string;

		artistString: string;

		id: number;

		localizedName: string;

		mainPicture: EntryThumbContract;

		ratingAverage: number;

		ratingCount: number;

		releaseDate: OptionalDateTimeContract;

	}

} 