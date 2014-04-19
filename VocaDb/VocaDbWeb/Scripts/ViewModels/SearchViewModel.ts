
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		constructor(albumRepo: rep.AlbumRepository, songRepo: rep.SongRepository) {

			this.albumSearchViewModel = new AlbumSearchViewModel(this, albumRepo);
			this.songSearchViewModel = new SongSearchViewModel(this, songRepo);

			this.searchTerm.subscribe(this.updateResults);

			this.searchType.subscribe(val => {

				if (this.currentSearchType() != "Anything") {
					this.updateResults();
				}
				this.currentSearchType(val);

			});

			this.updateResults();

		}

		public albumSearchViewModel: AlbumSearchViewModel;
		public songSearchViewModel: SongSearchViewModel;

		private currentSearchType = ko.observable("Anything");
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public searchType = ko.observable("Anything");

		public showAlbumSearch = ko.computed(() => this.searchType() == 'Album' || this.searchType() == 'Anything');
		public showSongSearch = ko.computed(() => this.searchType() == 'Song' || this.searchType() == 'Anything');

		public isUniversalSearch = ko.computed(() => this.searchType() == 'Anything');

		public updateResults = () => {
		
			if (this.showAlbumSearch())
				this.albumSearchViewModel.updateResultsWithTotalCount();
			
			if (this.showSongSearch())
				this.songSearchViewModel.updateResultsWithTotalCount();
				
		}

	}

	export class AlbumSearchViewModel {

		constructor(private searchViewModel: SearchViewModel, private albumRepo: rep.AlbumRepository) {

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		public page = ko.observableArray<dc.AlbumContract>([]);

		public paging = new ServerSidePagingViewModel();

		public sort = ko.observable("Name");

		public ratingStars = (album: dc.AlbumContract) => {

			if (!album)
				return [];

			var ratings = _.map([1, 2, 3, 4, 5], rating => { return { enabled: (Math.round(album.ratingAverage) >= rating) } });
			return ratings;

		};

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.albumRepo.getList(pagingProperties, this.searchViewModel.searchTerm(), this.sort(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);

			});

		};

	}

	export class SongSearchViewModel {

		constructor(private searchViewModel: SearchViewModel, private songRepo: rep.SongRepository) {

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		public page = ko.observableArray<dc.SongApiContract>([]);

		public paging = new ServerSidePagingViewModel();

		public sort = ko.observable("Name");

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.songRepo.getList(pagingProperties, this.searchViewModel.searchTerm(), this.sort(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);

			});

		};

	}

}