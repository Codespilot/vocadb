
ko.bindingHandlers.artistAutoComplete = {
    init: function (element, valueAccessor) {
        var properties = ko.utils.unwrapObservable(valueAccessor());

        initEntrySearch(element, null, "Artist", vdb.functions.mapAbsoluteUrl("/Artist/FindJson"), {
            allowCreateNew: properties.allowCreateNew,
            acceptSelection: properties.acceptSelection,
            createNewItem: properties.createNewItem,
            createOptionFirstRow: function (item) {
                return item.Name + " (" + item.ArtistType + ")";
            },
            createOptionSecondRow: function (item) {
                return item.AdditionalNames;
            },
            extraQueryParams: properties.extraQueryParams,
            filter: properties.filter,
            height: properties.height
        });
    }
};
