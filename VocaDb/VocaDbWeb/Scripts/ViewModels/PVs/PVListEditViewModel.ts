
module vdb.viewModels.pvs {

	import dc = vdb.dataContracts;
	
	export class PVListEditViewModel {

		constructor(private urlMapper: UrlMapper,
			pvs: dc.pvs.PVContract[]) {

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);
			this.pvs = ko.observableArray(_.map(pvs, pv => new PVEditViewModel(pv)));

		}

		public add = () => {

			var newPvUrl = this.newPvUrl();

			if (!newPvUrl)
				return;

			var pvType = this.newPvType();

			var url = this.urlMapper.mapRelative("/api/pvs");
			$.getJSON(url, { pvUrl: newPvUrl }, (pv: dc.pvs.PVContract) => {

				this.pvs.push(new PVEditViewModel(pv, pvType));

			});

		}

		public formatLength = (seconds: number) => {
			return vdb.helpers.DateTimeHelper.formatFromSeconds(seconds);
		}

		public getPvServiceIcon = (service: string) => {
			return this.pvServiceIcons.getIconUrl(service);
		}

		public newPvType = ko.observable("Original");

		public newPvUrl = ko.observable("");

		public pvs: KnockoutObservableArray<PVEditViewModel>;

		public pvServiceIcons: vdb.models.PVServiceIcons;

		public remove = (pv: PVEditViewModel) => {
			this.pvs.remove(pv);
		}

	}

}