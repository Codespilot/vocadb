/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	export class TagEditViewModel {

		public description: KnockoutObservable<string>;

		constructor(description: string) {

			this.description = ko.observable(description);

		}

	}

}