
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		updateResults = (clearResults: boolean) => {

			if (clearResults)
				this.albumsPaging.page(1);

			var pagingProperties = this.albumsPaging.getPagingProperties(clearResults);

			this.albumRepo.getList(pagingProperties, this.searchTerm(), this.albumSort(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.albumsPaging.totalItems(result.totalCount);

				this.albumsPage(result.items);

			});

		};

		updateResultsWithTotalCount = () => this.updateResults(true);
		updateResultsWithoutTotalCount = () => this.updateResults(false);

		constructor(private albumRepo: rep.AlbumRepository) {

			this.searchTerm.subscribe(this.updateResultsWithTotalCount);
			this.albumSort.subscribe(this.updateResultsWithTotalCount);
			this.albumsPaging.getItemsCallback = this.updateResultsWithoutTotalCount;
			//this.albumsPaging.pagingProperties.subscribe(this.updateResults);

			this.updateResultsWithTotalCount();

		}

		public albumsPage = ko.observableArray<dc.AlbumContract>([]);

		public albumsPaging = new ServerSidePagingViewModel();

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