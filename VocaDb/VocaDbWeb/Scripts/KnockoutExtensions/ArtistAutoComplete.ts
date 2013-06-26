/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.knockoutExtensions {

    export interface autoCompleteParams {
        
        acceptSelection: (id: number, term: string) => void;

        allowCreateNew: boolean;

        extraQueryParams;

        height: number;
    
    }

}

declare function initEntrySearch(nameBoxElem, findListElem, entityName: string, searchUrl: string, params);

interface KnockoutBindingHandlers {
    artistAutoComplete: KnockoutBindingHandler;
}

// Artist autocomplete search box.
ko.bindingHandlers.artistAutoComplete = {
    init: function (element, valueAccessor) {

        var properties: vdb.knockoutExtensions.autoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

        initEntrySearch(element, null, "Artist", vdb.functions.mapAbsoluteUrl("/Artist/FindJson"),
            {
                allowCreateNew: properties.allowCreateNew,
                acceptSelection: properties.acceptSelection,
                createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
                createOptionSecondRow: function (item) { return item.AdditionalNames; },
                extraQueryParams: properties.extraQueryParams,
                height: properties.height
            });


    }
}