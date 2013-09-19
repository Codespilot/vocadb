var vdb;
(function (vdb) {
    (function (viewModels) {
        var TopBarViewModel = (function () {
            function TopBarViewModel(entryTypeTranslations, entryType, searchTerm, hasUnreadMessages, getNewReportsCount, entryReportRepository) {
                var _this = this;
                this.reportCount = ko.observable(0);
                this.entryType = ko.observable(entryType);
                this.searchTerm = ko.observable(searchTerm);

                this.entryTypeName = ko.computed(function () {
                    return entryTypeTranslations[_this.entryType()];
                });

                this.hasNotifications = ko.computed(function () {
                    return (hasUnreadMessages || _this.reportCount() > 0);
                });

                if (getNewReportsCount) {
                    entryReportRepository.getNewReportCount(function (count) {
                        _this.reportCount(count);
                    });
                }
            }
            return TopBarViewModel;
        })();
        viewModels.TopBarViewModel = TopBarViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
