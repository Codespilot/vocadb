﻿
module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class EntryPictureFileEditViewModel {

		constructor(data?: dc.EntryPictureFileContract) {

			if (data) {
				this.entryType = data.entryType;
				this.id = data.id;
				this.mime = data.mime;
				this.thumbUrl = data.thumbUrl;
				this.name = ko.observable(data.name);				
			} else {
				this.name = ko.observable("");
			}

		}

		public entryType: string;
		public id: number;
		public mime: string;
		public name: KnockoutObservable<string>;
		public thumbUrl: string;

	}

}