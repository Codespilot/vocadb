
module vdb.viewModels {
	
	export class StatsViewModel {

		//public selectedReport: KnockoutObservable<IReport>;

		public selectedUrl: KnockoutObservable<string>;

		constructor(public categories: IReportCategory[]) {

			//this.selectedReport(categories[0].reports[0]);
			this.selectedUrl = ko.observable(categories[0].reports[0].url);

		}

	}

	export interface IReportCategory {

		name: string;

		reports: IReport;

	}

	export interface IReport {

		name: string;

		url: string;

	}

} 