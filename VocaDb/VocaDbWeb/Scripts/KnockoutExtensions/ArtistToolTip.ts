/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

interface KnockoutBindingHandlers {
    artistToolTip: KnockoutBindingHandler;
}

interface JQuery {
    qtip: (qtipProperties) => void;
}

/* 
    Displays artist tooltip.
    Binding value: artist Id.
*/
ko.bindingHandlers.artistToolTip = {
    init: function (element, valueAccessor) {

        var value = ko.utils.unwrapObservable(valueAccessor());

        $(element).qtip({
            content: {
                text: 'Loading...',
                ajax: {
                    url: vdb.functions.mapAbsoluteUrl('/Artist/PopupContent'),
                    type: 'GET',
                    data: { id: value }
                }
            },
            position: {
                viewport: $(window)
            },
            style: {
                classes: "tooltip-wide"
            }
        });

    }
};
