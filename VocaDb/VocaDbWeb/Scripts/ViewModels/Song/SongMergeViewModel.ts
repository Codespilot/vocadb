﻿
module vdb.viewModels.songs {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongMergeViewModel {
		
		constructor(songRepo: rep.SongRepository, id: number) {

			this.target = new BasicEntryLinkViewModel(null, songRepo.getOneBase);

			this.targetSearchParams = {
				acceptSelection: this.target.id,
				allowCreateNew: false,
				filter: (item) => item.Id != id
			};

		}

		public target: BasicEntryLinkViewModel<dc.SongContract>;
		public targetSearchParams: vdb.knockoutExtensions.AutoCompleteParams;

	}

}