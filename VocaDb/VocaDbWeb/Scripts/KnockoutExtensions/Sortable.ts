/// <reference path="../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />

interface KnockoutBindingHandlers {
    // Binding handler for jQuery focusout event.
    sortable: KnockoutBindingHandler;
}

// Enables JQuery UI sortable for the element.
// Parameters are passed as is.
ko.bindingHandlers.sortable = {
    init: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).sortable(value);
    }
};
