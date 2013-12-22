/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	// User my settings view model
	export class MySettingsViewModel {

		aboutMe: KnockoutObservable<string>;

		webLinksViewModel: WebLinksEditViewModel;

		constructor(aboutMe: string, webLinkContracts: dc.WebLinkContract[]) {

			this.aboutMe = ko.observable(aboutMe);
			this.webLinksViewModel = new WebLinksEditViewModel(webLinkContracts);

		}

	}

}