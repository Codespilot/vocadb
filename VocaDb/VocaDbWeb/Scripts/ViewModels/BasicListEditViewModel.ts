
module vdb.viewModels {
	
	export class BasicListEditViewModel<TItem, TContract> {
		
		constructor(private type: { new (contract?: TContract): TItem; },
			contracts: TContract[]) {

			this.items = ko.observableArray(_.map(contracts, contract => new type(contract)));

		}

		public add = () => {
			this.items.push(new this.type());
		}

		public items: KnockoutObservableArray<TItem>;

		public remove = (item: TItem) => {
			this.items.remove(item);
		}

		public toContracts: () => TContract[] = () => {
			return ko.toJS(this.items);
		}

	}

}