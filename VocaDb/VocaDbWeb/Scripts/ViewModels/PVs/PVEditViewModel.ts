﻿
module vdb.viewModels.pvs {

	import dc = vdb.dataContracts;

	export class PVEditViewModel {
		
		constructor(contract: dc.pvs.PVContract, pvType?: string) {

			this.author = contract.author;
			this.id = contract.id;
			this.length = contract.length;
			this.pvId = contract.pvId;
			this.service = contract.service;
			this.pvType = pvType || contract.pvType;
			this.thumbUrl = contract.thumbUrl;
			this.url = contract.url;

			this.name = ko.observable(contract.name);
			this.lengthFormatted = vdb.helpers.DateTimeHelper.formatFromSeconds(this.length);

		}

		author: string;

		id: number;

		length: number;

		lengthFormatted: string;

		name: KnockoutObservable<string>;

		pvId: string;

		service: string;

		pvType: string;

		thumbUrl: string;

		url: string;

	}

}