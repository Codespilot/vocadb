/// <reference path="../../typings/youtube/youtube.d.ts" />

module vdb.viewModels.songList {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class SongListPlayListViewModel {

		constructor(
			private urlMapper: UrlMapper,
			private songListRepo: rep.SongListRepository,
			private songRepo: rep.SongRepository,
			private userRepo: rep.UserRepository, 
			private languageSelection: string, 
			private listId: number) {

			this.hasMoreSongs = ko.computed(() => {
				return this.page().length < this.paging.totalItems();
			});

			this.selectedSong.subscribe(song => {

				if (song == null)
					return;

				if (this.autoplay()) {
					this.loadYoutubeVideo();
				} else {
					this.autoPlayPlayer = null;
					songRepo.pvPlayer(song.song.id, { elementId: "pv-player", enableScriptAccess: true }, result => {
						this.playerService = cls.pvs.PVService[result.pvService];
						this.playerHtml(result.playerHtml);
					});					
				}

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

			this.autoplay.subscribe(autoplay => {

				this.updateResults(true, () => {
					
					if (autoplay && !this.autoPlayPlayer) {

						if (this.playerService != cls.pvs.PVService.Youtube) {
							$("#pv-player-wrapper").empty();
							$("#pv-player-wrapper").append($("<div id='pv-player' />"));
						}

						// Youtube player object can only be created once per iframe, so reuse the existing player object if already created, otherwise create the player object.
						this.autoPlayPlayer = new YT.Player("pv-player", {
							events: {
								'onStateChange': (event: YT.EventArgs) => {

									// This will still be fired once if the user disabled autoplay mode.
									if (this.autoplay() && event.data == YT.PlayerState.ENDED) {
										this.nextSong();
									}

								},
								'onReady': () => {

									// If Youtube video wasn't playing, either load the Youtube video, or reset song if the current song doesn't have a Youtube PV.
									if (this.playerService != cls.pvs.PVService.Youtube) {

										if (this.selectedSong().song.pvServices.split(",").indexOf(cls.pvs.PVService[cls.pvs.PVService.Youtube]) > 0) {
											this.loadYoutubeVideo();
										} else {
											this.selectedSong(this.page()[0]);
										}

									}

								}
							}
						});

					}

				});

			});

		}

		public autoplay = ko.observable(false);
		private autoPlayPlayer: YT.Player = null;		
		public formatLength = (length: number) => vdb.helpers.DateTimeHelper.formatFromSeconds(length);

		private getSongIndex = (song: dc.songs.SongInListContract) => {
			
			for (var i = 0; i < this.page().length; ++i) {
				if (this.page()[i].song.id == song.song.id)
					return i;
			}

			return -1;

		}

		private hasMoreSongs: KnockoutComputed<boolean>;

		public isInit = false;

		public init = () => {

			if (this.isInit)
				return;

			this.updateResultsWithTotalCount();
			this.isInit = true;

		}

		private loadYoutubeVideo = () => {

			if (this.selectedSong() == null)
				return;

			$.getJSON(this.urlMapper.mapRelative("/api/songs/" + this.selectedSong().song.id + "/pvs"), { service: "Youtube" }, pvId => {

				if (this.autoPlayPlayer)
					this.autoPlayPlayer.loadVideoById(pvId);

			});
				
		}

		public nextSong = () => {

			var index = this.getSongIndex(this.selectedSong());

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
		private playerService: cls.pvs.PVService = null;

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
			var services = this.autoplay() ? "Youtube" : "Youtube,SoundCloud,NicoNicoDouga,Bilibili,Vimeo,Piapro";

			this.songListRepo.getSongs(this.listId, services, pagingProperties, this.languageSelection,
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