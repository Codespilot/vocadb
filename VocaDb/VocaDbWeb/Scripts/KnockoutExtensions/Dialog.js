
ko.bindingHandlers.dialog = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = ko.utils.unwrapObservable(valueAccessor()) || {};

        setTimeout(function () {
            options.close = function () {
                allBindingsAccessor().dialogVisible(false);
            };

            $(element).dialog(options);
        }, 0);

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).dialog("destroy");
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var shouldBeOpen = ko.utils.unwrapObservable(allBindingsAccessor().dialogVisible), $el = $(element), dialog = $el.data("uiDialog") || $el.data("dialog");

        var dialogButtons = ko.utils.unwrapObservable(allBindingsAccessor().dialogButtons);

        if (dialog) {
            $el.dialog("option", "buttons", dialogButtons);
            $el.dialog(shouldBeOpen ? "open" : "close");
        }
    }
};
