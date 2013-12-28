/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />

interface KnockoutBindingHandlers {

    jqButtonset: KnockoutBindingHandler;

}

// Adapted from http://therunningprogrammer.blogspot.fi/2011/10/how-to-use-jquery-uis-button-with.html and http://jsfiddle.net/photo_tom/hjk93/light/
ko.bindingHandlers.jqButtonset = {
    'init': function (element, valueAccessor, allBindingsAccessor) {

        var allbindings = allBindingsAccessor();
        var id = ko.utils.unwrapObservable<string>(allbindings.id);
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
                return; // "checked" binding only responds to checkboxes and selected radio buttons
            }

            var modelValue = valueAccessor();
            if ((element.type == "checkbox") && (ko.utils.unwrapObservable(modelValue) instanceof Array)) {
                // For checkboxes bound to an array, we add/remove the checkbox value to that array
                // This works for both observable and non-observable arrays
                var existingEntryIndex = ko.utils.arrayIndexOf(<any>ko.utils.unwrapObservable(modelValue), element.value);
                if (element.checked && (existingEntryIndex < 0)) modelValue.push(element.value);
                else if ((!element.checked) && (existingEntryIndex >= 0)) modelValue.splice(existingEntryIndex, 1);
            } else if (ko.isObservable(modelValue)) {
                if (modelValue() !== valueToWrite) { // Suppress repeated events when there's nothing new to notify (some browsers raise them)
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

        // IE 6 won't allow radio buttons to be selected unless they have a name
        //if ((element.type == "radio") && !element.name) ko.bindingHandlers.uniqueName.init(element, function () {
        //    return true
        //});
    },

    'update': function (element, valueAccessor) {
        /////////////// addded code to ko checked binding /////////////////
        var buttonSet = function (element) {

            // now update the css classes
            // Normally when knockout updates button, there
            // isn't an event to transfer new status
            // to buttonset label
            var buttonId = $(element).attr('id');
            if (buttonId) {
                var buttonSetDiv = $(element).parent('.ui-buttonset');
                var elementLabel = $(buttonSetDiv).find('label[for="' + buttonId + '"]');
                if (elementLabel.length === 0) {
                    // was just a single button, so look for label
                    elementLabel = $(element).parent('*').find('label[for="' + buttonId + '"]');
                }
                // check to see if element is already configured
                if (element.checked && !$(elementLabel).hasClass('ui-state-active')) {
                    $(elementLabel).addClass('ui-state-active');
                }
                if (!element.checked && $(elementLabel).hasClass('ui-state-active')) {
                    $(elementLabel).removeClass('ui-state-active');
                }
            }
        };
        /////////////// end add /////////////////////////// 
        var value = ko.utils.unwrapObservable(valueAccessor());

        if (element.type == "checkbox") {
            if (value instanceof Array) {
                // When bound to an array, the checkbox being checked represents its value being present in that array
                element.checked = ko.utils.arrayIndexOf(<any[]>value, element.value) >= 0;
            } else {
                // When bound to anything other value (not an array), the checkbox being checked represents the value being trueish
                element.checked = value;
            }
            /////////////// addded code to ko checked binding /////////////////
            buttonSet(element);
            /////////////// end add ///////////////////////////
            // Workaround for IE 6 bug - it fails to apply checked state to dynamically-created checkboxes if you merely say "element.checked = true"
            if (value && ko.utils.isIe6) element.mergeAttributes(document.createElement("<input type='checkbox' checked='checked' />"), false);
        } else if (element.type == "radio") {
            element.checked = (element.value == value);
            /////////////// addded code to ko checked binding /////////////////
            buttonSet(element);
            /////////////// end add ///////////////////////////
            // Workaround for IE 6/7 bug - it fails to apply checked state to dynamically-created radio buttons if you merely say "element.checked = true"
            if ((element.value == value) && (ko.utils.isIe6 || ko.utils.isIe7)) element.mergeAttributes(document.createElement("<input type='radio' checked='checked' />"), false);
        }
    }
};
