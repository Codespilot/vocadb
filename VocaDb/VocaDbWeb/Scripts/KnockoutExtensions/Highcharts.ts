/// <reference path="../typings/highcharts/highcharts.d.ts" />

interface KnockoutBindingHandlers {
	// Binding handler for jQuery focusout event.
	highcharts: KnockoutBindingHandler;
}

ko.bindingHandlers.highcharts = {
	init: (element: HTMLElement, valueAccessor) => {
		var func = ko.utils.unwrapObservable<Function>(valueAccessor());
		func((result: HighchartsOptions) => $(element).highcharts(result));
		//var url = ko.utils.unwrapObservable<string>(valueAccessor());
		//$.getJSON(url, (result: HighchartsOptions) => $(element).highcharts(result));
	}
};
