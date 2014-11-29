/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import hel = vdb.helpers;
	import rep = vdb.repositories;
	var SongType = cls.songs.SongType;

    export class SongEditViewModel {

        // List of artist links for this song.
        public artistLinks: KnockoutObservableArray<ArtistForAlbumEditViewModel>;
		artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
        public length: KnockoutObservable<number>;
		public lengthFormatted: KnockoutComputed<string>;
		public names: globalization.NamesEditViewModel;
		public pvs: pvs.PVListEditViewModel;
		public songType: KnockoutComputed<cls.songs.SongType>;
		public songTypeStr: KnockoutObservable<string>;
		public submitting = ko.observable(false);
		private tags: string[];
		public validationExpanded = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		// Adds a new artist to the album
		// artistId: Id of the artist being added, if it's an existing artist. Can be null, if custom artist.
		// customArtistName: Name of the custom artist being added. Can be null, if existing artist.
		addArtist = (artistId?: number, customArtistName?: string) => {

			if (artistId) {

				this.artistRepository.getOne(artistId, artist => {

					var data: dc.ArtistForAlbumContract = {
						artist: artist,
						isSupport: false,
						name: artist.name,
						id: 0,
						roles: 'Default'
					};

					var link = new ArtistForAlbumEditViewModel(null, data);
					this.artistLinks.push(link);

				});

			} else {

				var data: dc.ArtistForAlbumContract = {
					artist: null,
					name: customArtistName,
					isSupport: false,
					id: 0,
					roles: 'Default'
				};

				var link = new ArtistForAlbumEditViewModel(null, data);
				this.artistLinks.push(link);

			}

		};

		// Removes an artist from this album.
		public removeArtist = (artist: ArtistForAlbumEditViewModel) => {
			this.artistLinks.remove(artist);
		};

        public submit = () => {
            this.submitting(true);
            return true;
        }

		public translateArtistRole = (role: string) => {
			return this.artistRoleNames[role];
		};

		public hasValidationErrors: KnockoutComputed<boolean>;
		public validationError_needArtist: KnockoutComputed<boolean>;
		public validationError_needProducer: KnockoutComputed<boolean>;
		public validationError_needType: KnockoutComputed<boolean>;
		public validationError_nonInstrumentalSongNeedsVocalists: KnockoutComputed<boolean>;
		public validationError_unspecifiedNames: KnockoutComputed<boolean>;

		constructor(
			private artistRepository: rep.ArtistRepository,
			pvRepository: rep.PVRepository,
			urlMapper: vdb.UrlMapper,
			private artistRoleNames,
			webLinkCategories: vdb.dataContracts.TranslatedEnumField[],
			data: dc.songs.SongForEditContract,
			canBulkDeletePVs: boolean) {

			this.artistLinks = ko.observableArray(_.map(data.artistLinks, artist => new ArtistForAlbumEditViewModel(null, artist)));

			this.artistSearchParams = {
				createNewItem: vdb.resources.song.addExtraArtist,
				acceptSelection: this.addArtist,
				height: 300
			};

			this.length = ko.observable(data.length);
			this.names = globalization.NamesEditViewModel.fromContracts(data.names);
			this.pvs = new pvs.PVListEditViewModel(pvRepository, urlMapper, data.pvs, canBulkDeletePVs);
			this.songTypeStr = ko.observable(data.songType);
			this.songType = ko.computed(() => cls.songs.SongType[this.songTypeStr()]);
			this.tags = data.tags;
            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
            
            this.lengthFormatted = ko.computed({
				read: () => {
					return vdb.helpers.DateTimeHelper.formatFromSeconds(this.length());
                },
                write: (value: string) => {
                    var parts = value.split(":");
                    if (parts.length == 2 && parseInt(parts[0], 10) != NaN && parseInt(parts[1], 10) != NaN) {
                        this.length(parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10));
                    } else if (parts.length == 1 && !isNaN(parseInt(parts[0], 10))) {
                        this.length(parseInt(parts[0], 10));
                    } else {
                        this.length(0);
                    }
                }
			});

			this.validationError_needArtist = ko.computed(() => !_.some(this.artistLinks(), a => a.artist != null));
			this.validationError_needProducer = ko.computed(() => !this.validationError_needArtist() && !_.some(this.artistLinks(), a => a.artist != null && hel.ArtistHelper.isProducerRole(a.artist, a.rolesArray(), hel.SongHelper.isAnimation(this.songType()))));
			this.validationError_needType = ko.computed(() => this.songType() == SongType.Unspecified);

			this.validationError_nonInstrumentalSongNeedsVocalists = ko.computed(() => {

				return (!this.validationError_needArtist()
					&& !hel.SongHelper.isInstrumental(this.songType())
					&& !_.some(this.tags, t => t == cls.tags.Tag.commonTag_instrumental))
					&& !_.some(this.artistLinks(), a => hel.ArtistHelper.isVocalistRole(a.artist, a.rolesArray()));

			});

			this.validationError_unspecifiedNames = ko.computed(() => !this.names.hasNameWithLanguage());

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needArtist() ||
				this.validationError_needProducer() ||
				this.validationError_needType() ||
				this.validationError_nonInstrumentalSongNeedsVocalists() ||
				this.validationError_unspecifiedNames()
			);

        }

    }

}