﻿
module vdb.viewModels.globalization {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	var langSelection = vdb.models.globalization.ContentLanguageSelection;

	export class NamesEditViewModel {

		public aliases: KnockoutObservableArray<LocalizedStringWithIdEditViewModel>;
		public allNames: KnockoutComputed<LocalizedStringWithIdEditViewModel[]>;
		public englishName: LocalizedStringWithIdEditViewModel;
		public originalName: LocalizedStringWithIdEditViewModel;
		public romajiName: LocalizedStringWithIdEditViewModel;

		public createAlias = () => {
			this.aliases.push(new LocalizedStringWithIdEditViewModel());
		};

		public deleteAlias = (alias: LocalizedStringWithIdEditViewModel) => {
			this.aliases.remove(alias);
		};

		public hasNameWithLanguage = () => {
			return _.some(this.allNames(), (name: vdb.viewModels.globalization.LocalizedStringWithIdEditViewModel) => name.language() != langSelection.Unspecified);
		}

		public toContracts = () => {
			// TODO: ko.toJS?
			return _.map(this.allNames(), (name) => {

				var contract: dc.globalization.LocalizedStringWithIdContract = {
					id: name.id,
					language: name.languageStr(),
					value: name.value()
				}

				return contract;

			});
		}

		public static fromContracts(contracts: dc.globalization.LocalizedStringWithIdContract[]) {
			return new NamesEditViewModel(_.map(contracts, contract => globalization.LocalizedStringWithIdEditViewModel.fromContract(contract)));
		}

		private static nameOrEmpty(names: LocalizedStringWithIdEditViewModel[], lang: cls.globalization.ContentLanguageSelection) {

			var name = _.find(names, n => n.language() == lang);
			return name || new LocalizedStringWithIdEditViewModel(lang, "");

		}

		constructor(names: LocalizedStringWithIdEditViewModel[] = []) {

			this.englishName = NamesEditViewModel.nameOrEmpty(names, langSelection.English);
			this.originalName = NamesEditViewModel.nameOrEmpty(names, langSelection.Japanese);
			this.romajiName = NamesEditViewModel.nameOrEmpty(names, langSelection.Romaji);

			this.aliases = ko.observableArray(_.where(names, n => n.id != this.englishName.id && n.id != this.originalName.id && n.id != this.romajiName.id));

			this.allNames = ko.computed(() => _.filter([this.originalName, this.romajiName, this.englishName]
				.concat(this.aliases()), name => name != null && name.value != null && name.value() != null && name.value().length > 0));

		}

	}

} 