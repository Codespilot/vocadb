
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		constructor(entryRepo: rep.EntryRepository, artistRepo: rep.ArtistRepository, albumRepo: rep.AlbumRepository, songRepo: rep.SongRepository,
			resourceRepo: rep.ResourceRepository, cultureCode: string) {

			this.anythingSearchViewModel = new AnythingSearchViewModel(this, entryRepo);
			this.artistSearchViewModel = new ArtistSearchViewModel(this, artistRepo);
			this.albumSearchViewModel = new AlbumSearchViewModel(this, albumRepo, artistRepo);
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

	// Base class for different types of searches.
	export class SearchCategoryBaseViewModel<TEntry> {
		
		constructor(private searchViewModel: SearchViewModel) {

			this.paging.getItemsCallback = this.updateResultsWithoutTotalCount;

		}

		// Method for loading a page of results.
		public loadResults: (pagingProperties: dc.PagingProperties, searchTerm: string, tag: string,
			callback: (result: any) => void) => void;

		public loading = ko.observable(true); // Currently loading for data

		public page = ko.observableArray<dc.EntryContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(); // Paging view model

		// Update results loading the first page and updating total number of items.
		// Commonly this is done after changing the filters or sorting.
		public updateResultsWithTotalCount = () => this.updateResults(true);

		// Update a new page of results. Does not update total number of items.
		// This assumes the filters have not changed. Commonly this is done when paging.
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.loadResults(pagingProperties, this.searchViewModel.searchTerm(), this.searchViewModel.tag(), (result: any) => {

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

				this.page(result.items);
				this.loading(false);

			});

		};

	}

	export class AnythingSearchViewModel extends SearchCategoryBaseViewModel<dc.EntryContract> {

		constructor(searchViewModel: SearchViewModel, private entryRepo: rep.EntryRepository) {

			super(searchViewModel);

			this.loadResults = this.entryRepo.getList;

		}

		public entryUrl = (entry: dc.EntryContract) => {

			return vdb.utils.EntryUrlMapper.details(entry.entryType, entry.id);

		}

	}

	export class ArtistSearchViewModel extends SearchCategoryBaseViewModel<dc.ArtistApiContract> {

		constructor(searchViewModel: SearchViewModel, private artistRepo: rep.ArtistRepository) {

			super(searchViewModel);

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.artistType.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, callback) => {
				
				this.artistRepo.getList(pagingProperties, searchTerm, this.sort(), this.artistType(), tag, callback);

			}

		}

		public artistType = ko.observable("Unknown");
		public sort = ko.observable("Name");

	}

	export class AlbumSearchViewModel extends SearchCategoryBaseViewModel<dc.AlbumContract> {

		constructor(searchViewModel: SearchViewModel, private albumRepo: rep.AlbumRepository, private artistRepo: rep.ArtistRepository) {

			super(searchViewModel);

			var vm = this;

			this.artistSearchParams = {
				allowCreateNew: false,
				acceptSelection: (artistId: number) => {
					vm.artistId(artistId);
					this.artistRepo.getOne(artistId, artist => vm.artistName(artist.name));
				},
				height: 300
			};

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.albumType.subscribe(this.updateResultsWithTotalCount);
			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, callback) => {

				this.albumRepo.getList(pagingProperties, searchTerm, this.sort(), this.albumType(), tag, this.artistId(), this.artistParticipationStatus(), callback);

			}

		}

		public albumType = ko.observable("Unknown");
		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
		public sort = ko.observable("Name");

		public ratingStars = (album: dc.AlbumContract) => {

			if (!album)
				return [];

			var ratings = _.map([1, 2, 3, 4, 5], rating => { return { enabled: (Math.round(album.ratingAverage) >= rating) } });
			return ratings;

		};

	}

	export class SongSearchViewModel extends SearchCategoryBaseViewModel<dc.SongApiContract> {

		constructor(searchViewModel: SearchViewModel, private songRepo: rep.SongRepository) {

			super(searchViewModel);

			this.pvsOnly.subscribe(this.updateResultsWithTotalCount);
			this.songType.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, callback) => {

				this.songRepo.getList(pagingProperties, searchTerm, this.sort(), this.songType(), tag, this.pvsOnly(), callback);

			}

		}

		public pvsOnly = ko.observable(false);
		public songType = ko.observable("Unspecified");
		public sort = ko.observable("Name");

	}

}