﻿
module vdb.dataContracts {
	
	export interface AlbumContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		artistString: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

		ratingAverage: number;

		ratingCount: number;

		releaseDate: OptionalDateTimeContract;

	}

} 