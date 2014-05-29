
module vdb.viewModels.search {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		constructor(entryRepo: rep.EntryRepository, artistRepo: rep.ArtistRepository,
			albumRepo: rep.AlbumRepository, songRepo: rep.SongRepository,
			tagRepo: rep.TagRepository,
			resourceRepo: rep.ResourceRepository,
			userRepo: rep.UserRepository,
			languageSelection: string, cultureCode: string, searchType: string,
			searchTerm: string,
			tag: string,
			sort: string,
			artistId: number,
			artistType: string,
			albumType: string, songType: string, onlyWithPVs: boolean) {

			if (searchTerm)
				this.searchTerm(searchTerm);

			this.anythingSearchViewModel = new AnythingSearchViewModel(this, languageSelection, entryRepo);
			this.artistSearchViewModel = new ArtistSearchViewModel(this, languageSelection, artistRepo, artistType);
			this.albumSearchViewModel = new AlbumSearchViewModel(this, languageSelection, albumRepo, artistRepo, sort, artistId, albumType);
			this.songSearchViewModel = new SongSearchViewModel(this, languageSelection, songRepo, artistRepo, userRepo, sort, artistId, songType, onlyWithPVs);
			this.tagSearchViewModel = new TagSearchViewModel(this, tagRepo);

			if (tag || artistId != null || artistType || albumType || songType || onlyWithPVs != null)
				this.showAdvancedFilters(true);

			if (searchType)
				this.searchType(searchType);

			if (tag)
				this.tag(tag);

			this.pageSize.subscribe(this.updateResults);
			this.searchTerm.subscribe(this.updateResults);
			this.tag.subscribe(this.updateResults);
			this.draftsOnly.subscribe(this.updateResults);

			this.showAnythingSearch = ko.computed(() => this.searchType() == 'Anything');
			this.showArtistSearch = ko.computed(() => this.searchType() == 'Artist');
			this.showAlbumSearch = ko.computed(() => this.searchType() == 'Album');
			this.showSongSearch = ko.computed(() => this.searchType() == 'Song');

			this.searchType.subscribe(val => {

				this.updateResults();
				this.currentSearchType(val);

			});

			resourceRepo.getList(cultureCode, ['albumSortRuleNames', 'artistSortRuleNames', 'artistTypeNames', 'discTypeNames', 'songSortRuleNames', 'songTypeNames'], resources => {
				this.resources(resources);
				this.updateResults();
			});

		}

		public albumSearchViewModel: AlbumSearchViewModel;
		public anythingSearchViewModel: AnythingSearchViewModel;
		public artistSearchViewModel: ArtistSearchViewModel;
		public songSearchViewModel: SongSearchViewModel;
		public tagSearchViewModel: TagSearchViewModel;

		private currentSearchType = ko.observable("Anything");
		public draftsOnly = ko.observable(false);
		public pageSize = ko.observable(10);
		public resources = ko.observable<any>();
		public showAdvancedFilters = ko.observable(false);
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public searchType = ko.observable("Anything");
		public tag = ko.observable("");

		public showAnythingSearch: KnockoutComputed<boolean>;
		public showArtistSearch: KnockoutComputed<boolean>;
		public showAlbumSearch: KnockoutComputed<boolean>;
		public showSongSearch: KnockoutComputed<boolean>;
		public showTagSearch = ko.computed(() => this.searchType() == 'Tag');
		public showTagFilter = ko.computed(() => !this.showTagSearch());
		public showDraftsFilter = ko.computed(() => this.searchType() != 'Tag');

		public isUniversalSearch = ko.computed(() => this.searchType() == 'Anything');

		public currentCategoryViewModel = (): ISearchCategoryBaseViewModel => {
			
			switch (this.searchType()) {
				case 'Anything':
					return this.anythingSearchViewModel;
				case 'Artist':
					return this.artistSearchViewModel;
				case 'Album':
					return this.albumSearchViewModel;
				case 'Song':
					return this.songSearchViewModel;
				case 'Tag':
					return this.tagSearchViewModel;
				default:
					return null;
			}

		}

		public updateResults = () => {

			var vm = this.currentCategoryViewModel();

			if (vm != null)
				vm.updateResultsWithTotalCount();
				
		}

	}

}