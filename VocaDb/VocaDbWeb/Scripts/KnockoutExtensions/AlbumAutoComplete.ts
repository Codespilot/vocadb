﻿
interface KnockoutBindingHandlers {
	albumAutoComplete: KnockoutBindingHandler;
}

// Album autocomplete search box.
ko.bindingHandlers.albumAutoComplete = {
	init: (element: HTMLElement, valueAccessor) => {

		var properties: vdb.knockoutExtensions.AutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

		var filter = properties.filter;

		if (properties.ignoreId) {
			
			filter = (item: vdb.dataContracts.AlbumContract) => {

				if (properties.ignoreId && item.id == properties.ignoreId) {
					return false;
				}

				return properties.filter != null ? properties.filter(item) : true;

			}

		}

		var queryParams = { nameMatchMode: 'Auto' };
		if (properties.extraQueryParams)
			jQuery.extend(queryParams, properties.extraQueryParams);

		initEntrySearch(element, null, "Album", vdb.functions.mapAbsoluteUrl("/api/albums"),
			{
				allowCreateNew: properties.allowCreateNew,
				acceptSelection: properties.acceptSelection,
				createNewItem: properties.createNewItem,
				createCustomItem: properties.createCustomItem,
				createOptionFirstRow: (item: vdb.dataContracts.AlbumContract) => (item.name + " (" + item.discType + ")"),
				createOptionSecondRow: (item: vdb.dataContracts.AlbumContract) => (item.artistString),
				extraQueryParams: queryParams,
				filter: filter,
				height: properties.height,
				termParamName: 'query',
				method: 'GET'
			});


	}
}