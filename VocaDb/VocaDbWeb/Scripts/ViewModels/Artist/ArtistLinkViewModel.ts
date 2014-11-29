
module vdb.viewModels.artists {

	export class ArtistLinkViewModel {

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");

		public artistContract = ko.computed(() => {
			return {
				id: this.artistId(),
				name: this.artistName(),
				additionalNames: null,
				artistType: null
			}
		});

		public artistContractJson = ko.computed(() => ko.toJSON(this.artistContract()));

		public selectArtist = (selectedArtistId: number) => {
			this.artistId(selectedArtistId);
			this.repository.getOne(selectedArtistId, artist => this.artistName(artist.name));
		};

		static createFromContract = (repository: vdb.repositories.ArtistRepository, artistContract: vdb.dataContracts.ArtistContract) => {

			if (artistContract == null)
				return new ArtistLinkViewModel(repository);

			return new ArtistLinkViewModel(repository, artistContract.id, artistContract.name);

		}

		constructor(private repository: vdb.repositories.ArtistRepository, artistId?: number, artistName?: string) {

			this.artistId(artistId);
			this.artistName(artistName);

			if (artistId && !artistName)
				this.selectArtist(artistId);

		}

	}
	
}

