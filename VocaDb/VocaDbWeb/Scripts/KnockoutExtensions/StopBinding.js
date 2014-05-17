/// <reference path="../typings/knockout/knockout.d.ts" />

// Stops automatic Knockout binding for child elements.
// See http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
ko.bindingHandlers.stopBinding = {
    init: function () {
        return { controlsDescendantBindings: true };
    }
};
