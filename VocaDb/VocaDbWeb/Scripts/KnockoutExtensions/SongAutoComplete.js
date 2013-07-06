ko.bindingHandlers.songAutoComplete = {
    init: function (element, valueAccessor) {
        var properties = ko.utils.unwrapObservable(valueAccessor());

        initEntrySearch(element, null, "Song", vdb.functions.mapAbsoluteUrl("/Song/FindJsonByName"), {
            allowCreateNew: properties.allowCreateNew,
            acceptSelection: properties.acceptSelection,
            createOptionFirstRow: function (item) {
                return item.Name + " (" + item.SongType + ")";
            },
            createOptionSecondRow: function (item) {
                return item.ArtistString;
            },
            extraQueryParams: properties.extraQueryParams,
            height: properties.height
        });
    }
};
