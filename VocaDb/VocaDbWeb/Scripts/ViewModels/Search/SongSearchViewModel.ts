
module vdb.viewModels.search {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongSearchViewModel extends SearchCategoryBaseViewModel<dc.SongApiContract> {

		constructor(
			searchViewModel: SearchViewModel,
			lang: string,
			private songRepo: rep.SongRepository,
			private artistRepo: rep.ArtistRepository) {

			super(searchViewModel);

			var vm = this;

			this.artistSearchParams = {
				allowCreateNew: false,
				acceptSelection: (artistId: number) => {
					vm.artistId(artistId);
					this.artistRepo.getOne(artistId, artist => vm.artistName(artist.name));
				},
				height: 300
			};

			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.pvsOnly.subscribe(this.updateResultsWithTotalCount);
			this.since.subscribe(this.updateResultsWithTotalCount);
			this.songType.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				this.songRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.songType(), tag, this.artistId(),
					this.artistParticipationStatus(),
					this.pvsOnly(),
					this.since(),
					status, callback);

			}

		}

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
		public pvsOnly = ko.observable(false);
		public since = ko.observable<number>(null);
		public songType = ko.observable("Unspecified");
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.searchViewModel.resources() != null ? this.searchViewModel.resources().songSortRuleNames[this.sort()] : "");

	}

}