
ko.bindingHandlers.jqButton = {
    init: function (element, valueAccessor) {
        var params = ko.utils.unwrapObservable(valueAccessor());

        $(element).button({ disabled: params.disabled, icons: { primary: params.icon } });
    }
};
