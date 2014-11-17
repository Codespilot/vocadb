
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

			this.hasMoreSongs = ko.computed(() => {
				return this.page().length < this.paging.totalItems();
			});

			this.selectedSong.subscribe(song => {
				songRepo.pvPlayerWithRating(song.song.id, result => this.playerHtml(result.playerHtml));
			});

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			var elem = $(".songlist-playlist-songs");
			$(elem).scroll(() => {
				var element = elem[0];
				if (this.hasMoreSongs() && element.scrollHeight - element.scrollTop === element.clientHeight) {
					this.paging.nextPage();
					this.updateResultsWithoutTotalCount();
				}
			});

		}

		public formatLength = (length: number) => vdb.helpers.DateTimeHelper.formatFromSeconds(length);

		private hasMoreSongs: KnockoutComputed<boolean>;

		public isInit = false;

		public init = () => {

			if (this.isInit)
				return;

			this.updateResultsWithTotalCount();
			this.isInit = true;

		}

		public nextSong = () => {

			var index = this.page().indexOf(this.selectedSong());

			if (index + 1 < this.songsLoaded()) {
				this.selectedSong(this.page()[index + 1]);			
			} else {

				if (this.hasMoreSongs()) {
					this.paging.nextPage();
					this.updateResults(false, () => {
						this.selectedSong(this.page()[index + 1]);
					});					
				} else {
					this.selectedSong(this.page()[0]);					
				}

			}
			
		}

		public playerHtml = ko.observable<string>(null);

		public selectedSong = ko.observable<dc.songs.SongInListContract>(null);

		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.songs.SongInListContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(100); // Paging view model
		public pauseNotifications = false;
		public playListViewModel: SongListPlayListViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;

		public songsLoaded = ko.computed(() => this.page().length);

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean = true, callback?: () => void) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults) {
				this.page.removeAll();
				this.paging.page(1);				
			}

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

					if (callback)
						callback();

				});

		}

	}

}