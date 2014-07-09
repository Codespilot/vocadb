/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

    export class SongEditViewModel {

        // List of artist links for this song.
        public artistLinks: KnockoutObservableArray<ArtistForAlbumEditViewModel>;
		artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
        public length: KnockoutObservable<number>;
        public lengthFormatted: KnockoutComputed<string>;
        public submitting = ko.observable(false);
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

        private addLeadingZero(val) {
            return (val < 10 ? "0" + val : val);
        }

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

		constructor(private artistRepository: rep.ArtistRepository, private artistRoleNames, webLinkCategories: vdb.dataContracts.TranslatedEnumField[], data: SongEdit) {

			this.artistLinks = ko.observableArray(_.map(data.artistLinks, artist => new ArtistForAlbumEditViewModel(null, artist)));

			this.artistSearchParams = {
				createNewItem: vdb.resources.song.addExtraArtist,
				acceptSelection: this.addArtist,
				height: 300
			};

            this.length = ko.observable(data.length);
            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
            
            this.lengthFormatted = ko.computed({
                read: () => {
                    var mins = Math.floor(this.length() / 60);
                    return mins + ":" + this.addLeadingZero(this.length() % 60);
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

        }

    }

    export interface SongEdit {

		artistLinks: dc.ArtistForAlbumContract[];

        length: number;

        webLinks: vdb.dataContracts.WebLinkContract[];

    }

}