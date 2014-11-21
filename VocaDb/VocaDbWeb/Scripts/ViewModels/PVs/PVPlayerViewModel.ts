﻿
module vdb.viewModels.pvs {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;
	import serv = cls.pvs.PVService;

	export class PVPlayerViewModel {
		
		constructor(
			private urlMapper: UrlMapper,
			private songRepo: rep.SongRepository,
			private playerElementId: string,
			private wrapperElement: HTMLElement
			) {
			
			this.players = {
				Youtube: <IPVPlayer>new PVPlayerYoutube(playerElementId, wrapperElement, this.songFinishedPlayback),
				SoundCloud: <IPVPlayer>new PVPlayerSoundCloud(playerElementId, wrapperElement, this.songFinishedPlayback)
			};

			this.selectedSong.subscribe(song => {

				if (song == null) {

					if (this.currentPlayer) {
						this.currentPlayer.detach();
						this.currentPlayer = null;
					}

					this.playerHtml("");
					return;					
				}

				// Use current player
				if (this.currentPlayer && this.songHasPVService(song, this.currentPlayer.service)) {

					this.loadPVId(this.currentPlayer.service, song.song.id, this.currentPlayer.play);

				} else { 

					// Detech old player
					if (this.currentPlayer) {
						this.currentPlayer.detach();
						this.currentPlayer = null;						
					}

					// TODO: if autoplay mode is enabled, need to load a player that supports autoplay AND that has autoplay enabled

					// Load new player from server and attach it
					songRepo.pvPlayer(song.song.id, { elementId: playerElementId, enableScriptAccess: true }, result => {

						this.playerHtml(result.playerHtml);
						this.playerService = serv[result.pvService];
						this.currentPlayer = this.players[result.pvService];

						if (this.currentPlayer) {
							this.currentPlayer.attach(false, () => {
								this.currentPlayer.play();
							});
						}

					});

				}

			});

			this.autoplay.subscribe(autoplay => {

				if (autoplay) {
					
					/* 
						3 cases: 
						1) currently playing PV supports autoplay: no need to do anything (already attached)
						2) currently playing song has PV that supports autoplay with another player: switch player
						3) currently playing song doesn't have a PV that supports autoplay: switch song
					*/

					// Case 1
					if (this.currentPlayer) {
						return;
					}

					// Case 2
					var newService = _.find(this.autoplayServices, s => this.songHasPVService(this.selectedSong(), s));
					if (newService) {

						this.playerService = newService;
						this.currentPlayer = this.players[serv[newService]];
						this.currentPlayer.attach(true, () => {
							this.loadPVId(this.currentPlayer.service, this.selectedSong().song.id, this.currentPlayer.play);
						});
						
					}

					// Case 3
					if (this.resetSong)
						this.resetSong();

				}

			});

		}

		public autoplay = ko.observable(false);
		private autoplayServices = [serv.Youtube, serv.SoundCloud];
		private currentPlayer: IPVPlayer = null;

		private loadPVId = (service: serv, songId: number, callback: (pvId: string) => void) => {

			$.getJSON(this.urlMapper.mapRelative("/api/songs/" + songId + "/pvs"), { service: serv[service] }, callback);

		}

		private players: { [index: string]: IPVPlayer; };
		public nextSong: () => void;
		public playerHtml = ko.observable<string>(null);
		public playerService: serv = null;
		public resetSong: () => void = null;
		public selectedSong = ko.observable<IPVPlayerSong>(null);
		private static serviceName = (service: serv) => serv[service];

		private songFinishedPlayback = () => {

			if (this.autoplay() && this.nextSong)
				this.nextSong();

		}

		private songHasPVService = (song: IPVPlayerSong, service: serv) => {
			return _.contains(song.song.pvServicesArray, service);
		}

		public songIsValid = (song: IPVPlayerSong) => {
			return !this.autoplay() || this.autoplayServices.some(s => _.contains(song.song.pvServicesArray, s));
		}

	}

	export interface IPVPlayerSong {

		song: dc.SongApiContract;

	}

	export interface IPVPlayer {

		// Attach the player by creating the JavaScript object, either to the currently playing element, or create a new element.
		// reset: whether to create a new player element
		// readyCallback: called when the player is ready
		attach: (reset?: boolean, readyCallback?: () => void) => void;

		detach: () => void;

		// Called when the currently playing song has finished playing. This will only be called if the player was attached.
		songFinishedCallback?: () => void;
		play: (pvId?: string) => void;
		service: cls.pvs.PVService;

	}

}