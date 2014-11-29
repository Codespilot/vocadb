/// <reference path="../../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../../DataContracts/WebLinkContract.ts" />
/// <reference path="../WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

    export class ArtistEditViewModel {

		private addGroup = (artistId: number) => {
			
			if (artistId) {
				this.artistRepo.getOne(artistId, (artist: dc.ArtistContract) => {
					this.groups.push({ id: 0, group: artist });
				});
			}

		}

		public artistType: KnockoutComputed<cls.artists.ArtistType>;
		public artistTypeStr: KnockoutObservable<string>;
		public allowBaseVoicebank: KnockoutComputed<boolean>;
		public baseVoicebank: artists.ArtistLinkViewModel;
		public baseVoicebankSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
		public canHaveCircles: KnockoutComputed<boolean>;
		public defaultNameLanguage: KnockoutObservable<string>;
		public description: KnockoutObservable<string>;
		public groups: KnockoutObservableArray<dc.artists.GroupForArtistContract>;

		public groupSearchParams: vdb.knockoutExtensions.AutoCompleteParams = {
			allowCreateNew: false,
			acceptSelection: this.addGroup,
			extraQueryParams: { artistTypes: "Label,Circle,OtherGroup,Band" },
			height: 300
		};

		public hasValidationErrors: KnockoutComputed<boolean>;
		public id: number;

		public names: globalization.NamesEditViewModel;
		public pictures: EntryPictureFileListEditViewModel;

		public removeGroup = (group: dc.artists.GroupForArtistContract) => {
			this.groups.remove(group);
		}

		public status: KnockoutObservable<string>;

		public submit = () => {

			this.submitting(true);

			var submittedModel: dc.artists.ArtistForEditContract = {
				artistType: this.artistTypeStr(),
				baseVoicebank: this.baseVoicebank.artistContract(),
				defaultNameLanguage: this.defaultNameLanguage(),
				description: this.description(),
				groups: this.groups(),
				id: this.id,
				names: this.names.toContracts(),
				pictures: this.pictures.toContracts(),
				status: this.status(),
				updateNotes: this.updateNotes(),
				webLinks: this.webLinks.toContracts(),
				additionalNames: "",
				pictureMime: ""
			};

			this.submittedJson(ko.toJSON(submittedModel));

			return true;

		}

		public submittedJson = ko.observable("");

		public submitting = ko.observable(false);
		public updateNotes = ko.observable("");
		public validationExpanded = ko.observable(false);
		public validationError_needReferences: KnockoutComputed<boolean>;
		public validationError_needType: KnockoutComputed<boolean>;
		public validationError_unspecifiedNames: KnockoutComputed<boolean>;
        public webLinks: WebLinksEditViewModel;

        constructor(private artistRepo: rep.ArtistRepository, webLinkCategories: vdb.dataContracts.TranslatedEnumField[], data: dc.artists.ArtistForEditContract) {

			this.artistTypeStr = ko.observable(data.artistType);
			this.artistType = ko.computed(() => cls.artists.ArtistType[this.artistTypeStr()]);
			this.allowBaseVoicebank = ko.computed(() => helpers.ArtistHelper.isVocalistType(this.artistType()) || this.artistType() == cls.artists.ArtistType.OtherIndividual);
			this.baseVoicebank = viewModels.artists.ArtistLinkViewModel.createFromContract(artistRepo, data.baseVoicebank);
			this.description = ko.observable(data.description);
			this.defaultNameLanguage = ko.observable(data.defaultNameLanguage);
			this.groups = ko.observableArray(data.groups);
			this.id = data.id;
			this.names = globalization.NamesEditViewModel.fromContracts(data.names);
			this.pictures = new EntryPictureFileListEditViewModel(data.pictures);
			this.status = ko.observable(data.status);
            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
    
			this.baseVoicebankSearchParams = {
				acceptSelection: this.baseVoicebank.selectArtist,
				extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,OtherVoiceSynthesizer,Unknown" },
				filter: (item) => item.Id != this.id,
			};

			this.canHaveCircles = ko.computed(() => {
				return this.artistType() != cls.artists.ArtistType.Label && this.artistType() != cls.artists.ArtistType.Circle;
			});

			this.validationError_needReferences = ko.computed(() => (this.description() == null || this.description().length) == 0 && this.webLinks.webLinks().length == 0);
			this.validationError_needType = ko.computed(() => this.artistType() == cls.artists.ArtistType.Unknown);
			this.validationError_unspecifiedNames = ko.computed(() => !this.names.hasNameWithLanguage());

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needReferences() ||
				this.validationError_needType() ||
				this.validationError_unspecifiedNames()
			);
			    
        }

    }

}