/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />

ko.bindingHandlers.jqButton = {
    init: function (element, valueAccessor) {
        var params = ko.utils.unwrapObservable(valueAccessor());

        $(element).button({ disabled: params.disabled, icons: { primary: params.icon } });
    }
};
