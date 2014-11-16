
module vdb.viewModels.songList {
	
	import dc = vdb.dataContracts;

	export class SongListPlayListViewModel {

		constructor(
			urlMapper: UrlMapper,
			private songListRepo: rep.SongListRepository,
			private songRepo: rep.SongRepository,
			private userRepo: rep.UserRepository, 
			private languageSelection: string, 
			private listId: number) {

			this.selectedSong.subscribe(song => {
				songRepo.pvPlayerWithRating(song.song.id, result => this.playerHtml(result.playerHtml));
			});

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);

			var elem = $(".songlist-playlist-songs");
			$(elem).scroll(() => {
				var element = elem[0];
				if (element.scrollHeight - element.scrollTop === element.clientHeight) {
					this.paging.nextPage();
				}
			});

			this.updateResultsWithTotalCount();

		}

		public formatLength = (length: number) => vdb.helpers.DateTimeHelper.formatFromSeconds(length);

		public playerHtml = ko.observable<string>(null);

		public selectedSong = ko.observable<dc.songs.SongInListContract>(null);

		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.songs.SongInListContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(100); // Paging view model
		public pauseNotifications = false;
		public playListViewModel: SongListPlayListViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;

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

			this.songListRepo.getSongs(this.listId, "Youtube,SoundCloud,NicoNicoDouga,Bilibili,Vimeo,Piapro", pagingProperties, this.languageSelection,
				(result: dc.PartialFindResultContract<dc.songs.SongInListContract>) => {

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					_.each(result.items, item => {
						this.page.push(item);
					});
					
					this.loading(false);

					if (result.items && result.items.length && !this.selectedSong())
						this.selectedSong(result.items[0]);

				});

		}

	}

}