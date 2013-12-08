
ko.bindingHandlers.jqButtonset = {
    'init': function (element, valueAccessor, allBindingsAccessor) {
        var allbindings = allBindingsAccessor();
        var id = ko.utils.unwrapObservable(allbindings.id);
        $(element).attr("id", id);
        $(element).find("~label").attr("for", id);
        $(element).button();

        var updateHandler = function () {
            var valueToWrite;
            if (element.type == "checkbox") {
                valueToWrite = element.checked;
            } else if ((element.type == "radio") && (element.checked)) {
                valueToWrite = element.value;
            } else {
                return;
            }

            var modelValue = valueAccessor();
            if ((element.type == "checkbox") && (ko.utils.unwrapObservable(modelValue) instanceof Array)) {
                var existingEntryIndex = ko.utils.arrayIndexOf(ko.utils.unwrapObservable(modelValue), element.value);
                if (element.checked && (existingEntryIndex < 0))
                    modelValue.push(element.value);
                else if ((!element.checked) && (existingEntryIndex >= 0))
                    modelValue.splice(existingEntryIndex, 1);
            } else if (ko.isObservable(modelValue)) {
                if (modelValue() !== valueToWrite) {
                    modelValue(valueToWrite);
                }
            } else {
                var allBindings = allBindingsAccessor();
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['checked']) {
                    allBindings['_ko_property_writers']['checked'](valueToWrite);
                }
            }
        };
        ko.utils.registerEventHandler(element, "click", updateHandler);
    },
    'update': function (element, valueAccessor) {
        var buttonSet = function (element) {
            var buttonId = $(element).attr('id');
            if (buttonId) {
                var buttonSetDiv = $(element).parent('.ui-buttonset');
                var elementLabel = $(buttonSetDiv).find('label[for="' + buttonId + '"]');
                if (elementLabel.length === 0) {
                    elementLabel = $(element).parent('*').find('label[for="' + buttonId + '"]');
                }

                if (element.checked && !$(elementLabel).hasClass('ui-state-active')) {
                    $(elementLabel).addClass('ui-state-active');
                }
                if (!element.checked && $(elementLabel).hasClass('ui-state-active')) {
                    $(elementLabel).removeClass('ui-state-active');
                }
            }
        };

        var value = ko.utils.unwrapObservable(valueAccessor());

        if (element.type == "checkbox") {
            if (value instanceof Array) {
                element.checked = ko.utils.arrayIndexOf(value, element.value) >= 0;
            } else {
                element.checked = value;
            }

            buttonSet(element);

            if (value && ko.utils.isIe6)
                element.mergeAttributes(document.createElement("<input type='checkbox' checked='checked' />"), false);
        } else if (element.type == "radio") {
            element.checked = (element.value == value);

            buttonSet(element);

            if ((element.value == value) && (ko.utils.isIe6 || ko.utils.isIe7))
                element.mergeAttributes(document.createElement("<input type='radio' checked='checked' />"), false);
        }
    }
};
