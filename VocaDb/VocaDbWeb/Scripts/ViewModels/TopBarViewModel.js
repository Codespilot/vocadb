var vdb;
(function (vdb) {
    (function (viewModels) {
        var TopBarViewModel = (function () {
            function TopBarViewModel(entryTypeTranslations, entryType, searchTerm) {
                var _this = this;
                this.entryType = ko.observable(entryType);
                this.searchTerm = ko.observable(searchTerm);

                this.entryTypeName = ko.computed(function () {
                    return entryTypeTranslations[_this.entryType()];
                });
            }
            return TopBarViewModel;
        })();
        viewModels.TopBarViewModel = TopBarViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
