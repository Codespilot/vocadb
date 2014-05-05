
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		constructor(entryRepo: rep.EntryRepository, artistRepo: rep.ArtistRepository, albumRepo: rep.AlbumRepository, songRepo: rep.SongRepository,
			resourceRepo: rep.ResourceRepository, cultureCode: string) {

			this.anythingSearchViewModel = new AnythingSearchViewModel(this, entryRepo);
			this.artistSearchViewModel = new ArtistSearchViewModel(this, artistRepo);
			this.albumSearchViewModel = new AlbumSearchViewModel(this, albumRepo);
			this.songSearchViewModel = new SongSearchViewModel(this, songRepo);

			this.searchTerm.subscribe(this.updateResults);

			this.searchType.subscribe(val => {

				this.updateResults();
				this.currentSearchType(val);

			});

			this.tag.subscribe(this.updateResults);

			this.showAnythingSearch = ko.computed(() => this.searchType() == 'Anything');
			this.showArtistSearch = ko.computed(() => this.searchType() == 'Artist');
			this.showAlbumSearch = ko.computed(() => this.searchType() == 'Album');
			this.showSongSearch = ko.computed(() => this.searchType() == 'Song');

			resourceRepo.getList(cultureCode, ['artistTypeNames', 'discTypeNames', 'songTypeNames'], resources => {
				this.resources = resources;
				this.updateResults();
			});

		}

		public albumSearchViewModel: AlbumSearchViewModel;
		public anythingSearchViewModel: AnythingSearchViewModel;
		public artistSearchViewModel: ArtistSearchViewModel;
		public songSearchViewModel: SongSearchViewModel;

		private currentSearchType = ko.observable("Anything");
		private resources;
		public showAdvancedFilters = ko.observable(false);
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public searchType = ko.observable("Anything");
		public tag = ko.observable("");

		public updateAnythingSearch = ko.computed(() => this.searchType() == 'Anything');
		public updateArtistSearch = ko.computed(() => this.searchType() == 'Artist');
		public updateAlbumSearch = ko.computed(() => this.searchType() == 'Album');
		public updateSongSearch = ko.computed(() => this.searchType() == 'Song');

		public showAnythingSearch: KnockoutComputed<boolean>;
		public showArtistSearch: KnockoutComputed<boolean>;
		public showAlbumSearch: KnockoutComputed<boolean>;
		public showSongSearch: KnockoutComputed<boolean>;

		public isUniversalSearch = ko.computed(() => this.searchType() == 'Anything');

		public updateResults = () => {

			if (this.updateAnythingSearch())
				this.anythingSearchViewModel.updateResultsWithTotalCount();

			if (this.updateArtistSearch())
				this.artistSearchViewModel.updateResultsWithTotalCount();
		
			if (this.updateAlbumSearch())
				this.albumSearchViewModel.updateResultsWithTotalCount();
			
			if (this.updateSongSearch())
				this.songSearchViewModel.updateResultsWithTotalCount();
				
		}

	}

	export class AnythingSearchViewModel {

		constructor(private searchViewModel: SearchViewModel, private entryRepo: rep.EntryRepository) {

			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		public entryUrl = (entry: dc.EntryContract) => {

			return vdb.utils.EntryUrlMapper.details(entry.entryType, entry.id);

		}

		public loading = ko.observable(true);

		public page = ko.observableArray<dc.EntryContract>([]);

		public paging = new ServerSidePagingViewModel();

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.entryRepo.getList(pagingProperties, this.searchViewModel.searchTerm(), this.searchViewModel.tag(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);
				this.loading(false);

			});

		};

	}

	export class ArtistSearchViewModel {

		constructor(private searchViewModel: SearchViewModel, private artistRepo: rep.ArtistRepository) {

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.artistType.subscribe(this.updateResultsWithTotalCount);
			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		public artistType = ko.observable("Nothing");
		public loading = ko.observable(true);
		public page = ko.observableArray<dc.ArtistApiContract>([]);

		public paging = new ServerSidePagingViewModel();

		public sort = ko.observable("Name");

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.artistRepo.getList(pagingProperties, this.searchViewModel.searchTerm(), this.sort(), this.artistType(), this.searchViewModel.tag(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);
				this.loading(false);

			});

		};

	}

	export class AlbumSearchViewModel {

		constructor(private searchViewModel: SearchViewModel, private albumRepo: rep.AlbumRepository) {

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		public loading = ko.observable(true);
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

			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.albumRepo.getList(pagingProperties, this.searchViewModel.searchTerm(), this.sort(), this.searchViewModel.tag(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);
				this.loading(false);

			});

		};

	}

	export class SongSearchViewModel {

		constructor(private searchViewModel: SearchViewModel, private songRepo: rep.SongRepository) {

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		public loading = ko.observable(true);
		public page = ko.observableArray<dc.SongApiContract>([]);

		public paging = new ServerSidePagingViewModel();

		public sort = ko.observable("Name");

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.songRepo.getList(pagingProperties, this.searchViewModel.searchTerm(), this.sort(), this.searchViewModel.tag(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);
				this.loading(false);

			});

		};

	}

}