
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		updateResults = () => {

			this.albumRepo.getList(0, this.searchTerm(), (result: any) => {

				this.albumsPage(result.items);

			});

		};

		constructor(private albumRepo: rep.AlbumRepository) {

			this.searchTerm.subscribe(this.updateResults);

			this.updateResults();

		}

		public albumsPage = ko.observableArray<dc.AlbumContract>([]);
		public searchTerm = ko.observable("").extend({ rateLimit: 400 });
		public searchType = ko.observable("Anything");

	}

}