
interface KnockoutBindingHandlers {
	tagAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.tagAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any) => {

		$(element).autocomplete({
			source: (ui, callback) => {
				$.getJSON(vdb.functions.mapAbsoluteUrl("/api/tags/names"), { query: ui.term }, callback);
			},
			select: (event, ui) => {
				valueAccessor()(ui.item.label);
				$(element).val("");
				return false;
			}
		});

	}
}