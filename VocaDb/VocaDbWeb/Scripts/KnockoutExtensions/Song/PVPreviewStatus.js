ko.bindingHandlers.pvPreviewStatus = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var urlMapper = new vdb.UrlMapper(vdb.values.baseAddress);
        var songRepository = new vdb.repositories.SongRepository(vdb.values.baseAddress);
        var userRepository = new vdb.repositories.UserRepository(urlMapper);
        var pvRows = $(element).find(".js-pvRow");
        var songsArray = valueAccessor();

        var songItems = _.map(pvRows, function (pvRow) {
            var songId = $(pvRow).data("entry-id");

            var item = new vdb.viewModels.SongWithPreviewViewModel(songRepository, userRepository, songId);
            item.ratingComplete = vdb.ui.showThankYouForRatingMessage;

            var childBindingContext = bindingContext.createChildContext(item);
            ko.applyBindingsToDescendants(childBindingContext, pvRow);
            return item;
        });

        songsArray(songItems);

        return { controlsDescendantBindings: true };
    }
};
