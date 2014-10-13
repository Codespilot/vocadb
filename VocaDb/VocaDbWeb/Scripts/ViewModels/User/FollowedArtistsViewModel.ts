
module vdb.viewModels.user {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class FollowedArtistsViewModel {

		constructor(private userRepo: rep.UserRepository, private languageSelection: string, private loggedUserId: number) {

			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);

		}

		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.RatedSongForUserForApiContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;

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

			this.userRepo.getFollowedArtistsList(this.loggedUserId, pagingProperties, this.languageSelection,
				(result: any) => {

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(result.items);
					this.loading(false);

				});

		}

	}

}