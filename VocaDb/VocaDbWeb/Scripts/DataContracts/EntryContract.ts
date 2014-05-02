
module vdb.dataContracts {
	
	// Base data contract for entries from the API.
	// Corresponds to C# datacontract EntryForApiContract.
	export interface EntryContract {

		additionalNames: string;

		entryType: string;

		id: number;

		localizedName: string;

		mainPicture: EntryThumbContract;

	}

}