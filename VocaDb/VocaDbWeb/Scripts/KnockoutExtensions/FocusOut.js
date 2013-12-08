
ko.bindingHandlers.focusout = {
    init: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).focusout(value);
    }
};
