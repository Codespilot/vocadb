ko.bindingHandlers.sortable = {
    init: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).sortable(value);
    }
};
