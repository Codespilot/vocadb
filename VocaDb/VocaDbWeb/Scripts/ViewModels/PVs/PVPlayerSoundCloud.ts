﻿/// <reference path="../../typings/soundcloud/soundcloud.d.ts" />

module vdb.viewModels.pvs {

	import cls = vdb.models;

	export class PVPlayerSoundCloud implements IPVPlayer {

		constructor(
			private playerElementId: string,
			private wrapperElement: HTMLElement,
			public songFinishedCallback: () => void = null) {

		}

		public attach = (reset: boolean = false, readyCallback?: () => void) => {

			if (!reset && this.player) {
				if (readyCallback)
					readyCallback();
				return;				
			}

			if (reset) {
				$(this.wrapperElement).empty();
				$(this.wrapperElement).append($("<div id='" + this.playerElementId + "' src='" + location.protocol + "//w.soundcloud.com/player/' />"));
			}

			this.player = SC.Widget(this.playerElementId);
			this.player.bind(SC.Widget.Events.FINISH, () => {

				if (this.player && this.songFinishedCallback)
					this.songFinishedCallback();

			});

			this.player.bind(SC.Widget.Events.READY, () => {

				if (readyCallback)
					readyCallback();

			});

		}

		public detach = () => {

			if (this.player) {
				this.player.unbind(SC.Widget.Events.FINISH);
			}

			this.player = null;
		}

		private player: SC.SoundCloudWidget = null;

		public play = (pvId?: string) => {

			if (!this.player)
				this.attach(false);

			if (pvId) {
				this.player.load(pvId, { auto_play: true });
			} else {
				this.player.play();
				
			}

		}

		public service = cls.pvs.PVService.SoundCloud;

	}

}