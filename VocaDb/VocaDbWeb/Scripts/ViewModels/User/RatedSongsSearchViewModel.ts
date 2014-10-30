﻿
module vdb.viewModels.user {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class RatedSongsSearchViewModel {
		
		constructor(private userRepo: rep.UserRepository, private artistRepo: rep.ArtistRepository,
			private songRepo: rep.SongRepository,
			private resourceRepo: rep.ResourceRepository,
			private languageSelection: string, private loggedUserId: number, private cultureCode: string,
			sort: string, groupByRating: boolean,
			initialize = true) {

			if (sort)
				this.sort(sort);

			if (groupByRating != null)
				this.groupByRating(groupByRating);

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
			this.tag.subscribe(this.updateResultsWithTotalCount);

			if (initialize)
				this.init();

		}

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
		public groupByRating = ko.observable(true);
		public isInit = false;
		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.RatedSongForUserForApiContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public rating = ko.observable("Nothing");
		public resources = ko.observable<any>();
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public songListId = ko.observable<number>(undefined);
		public songLists = ko.observableArray<dc.SongListBaseContract>([]);
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.resources() != null ? this.resources().songSortRuleNames[this.sort()] : "");
		public tag = ko.observable("");

		public init = () => {

			if (this.isInit)
				return;

			this.userRepo.getSongLists(this.loggedUserId, songLists => this.songLists(songLists));

			this.resourceRepo.getList(this.cultureCode, ['songSortRuleNames', 'songTypeNames'], resources => {
				this.resources(resources);
				this.updateResultsWithTotalCount();
				this.isInit = true;
			});

		};

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

			this.userRepo.getRatedSongsList(this.loggedUserId, pagingProperties, this.languageSelection, this.searchTerm(),
				this.tag(),
				this.artistId(),
				this.rating(), this.songListId(), this.groupByRating(), this.sort(),
				(result: any) => {

					_.each(result.items, (item: dc.RatedSongForUserForApiContract) => {

						var song = item.song;
						var songAny: any = song;

						if (song.pvServices && song.pvServices != 'Nothing') {
							songAny.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id, song.pvServices);
							songAny.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
						} else {
							songAny.previewViewModel = null;
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