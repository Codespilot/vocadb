
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		updateResults = () => {

			this.albumRepo.getList(0, this.searchTerm(), this.albumSort(), (result: any) => {

				this.albumsPage(result.items);

			});

		};

		constructor(private albumRepo: rep.AlbumRepository) {

			this.searchTerm.subscribe(this.updateResults);
			this.albumSort.subscribe(this.updateResults);

			this.updateResults();

		}

		public albumsPage = ko.observableArray<dc.AlbumContract>([]);

		public ratingStars = (album: dc.AlbumContract) => {

			if (!album)
				return [];

			var ratings = _.map([1, 2, 3, 4, 5], rating => { return { enabled: (Math.round(album.ratingAverage) >= rating) } });
			return ratings;

		};

		public albumSort = ko.observable("Name");
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public searchType = ko.observable("Anything");

	}

}