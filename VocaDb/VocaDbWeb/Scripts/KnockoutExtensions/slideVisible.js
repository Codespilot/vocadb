ko.bindingHandlers.slideVisible = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.utils.unwrapObservable(value));
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
        ko.utils.unwrapObservable(value) ? $(element).slideDown('fast') : $(element).slideUp('fast', function () {
            return allBindingsAccessor().complete(ko.dataFor(element));
        });
    }
};
