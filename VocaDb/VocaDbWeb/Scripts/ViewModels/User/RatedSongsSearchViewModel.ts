
module vdb.viewModels.user {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class RatedSongsSearchViewModel {
		
		constructor(private userRepo: rep.UserRepository, private artistRepo: rep.ArtistRepository,
			private songRepo: rep.SongRepository,
			resourceRepo: rep.ResourceRepository,
			private languageSelection: string, private loggedUserId: number, cultureCode: string) {
			
			this.artistSearchParams = {
				allowCreateNew: false,
				acceptSelection: this.selectArtist,
				height: 300
			};

			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.groupByRating.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);
			this.rating.subscribe(this.updateResultsWithTotalCount);
			this.searchTerm.subscribe(this.updateResultsWithTotalCount);
			this.songListId.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithoutTotalCount);

			userRepo.getSongLists(loggedUserId, songLists => this.songLists(songLists));

			resourceRepo.getList(cultureCode, ['songSortRuleNames', 'songTypeNames'], resources => {
				this.resources(resources);
				this.updateResultsWithTotalCount();
			});

		}

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
		public groupByRating = ko.observable(true);
		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.RatedSongForUserForApiContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public rating = ko.observable("Nothing");
		public resources = ko.observable<any>();
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public songListId = ko.observable<number>(null);
		public songLists = ko.observableArray<dc.SongListBaseContract>([]);
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.resources() != null ? this.resources().songSortRuleNames[this.sort()] : "");

		public selectArtist = (selectedArtistId: number) => {
			this.artistId(selectedArtistId);
			this.artistRepo.getOne(selectedArtistId, artist => this.artistName(artist.name));
		};

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean = true) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.userRepo.getRatedSongsList(this.loggedUserId, pagingProperties, this.languageSelection, this.searchTerm(), this.artistId(),
				this.rating(), this.songListId(), this.groupByRating(), this.sort(),
				(result: any) => {

					_.each(result.items, (item: any) => {

						var song = item.song;

						if (song.pvServices && song.pvServices != 'Nothing') {
							song.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id);
							song.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
						} else {
							song.previewViewModel = null;
						}

					});

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(result.items);
					this.loading(false);

				});

		}

	}

}