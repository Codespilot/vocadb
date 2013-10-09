var vdb;
(function (vdb) {
    (function (viewModels) {
        

        var TopBarViewModel = (function () {
            function TopBarViewModel(entryTypeTranslations, entryType, searchTerm, unreadMessagesCount, getNewReportsCount, entryReportRepository, userRepository) {
                var _this = this;
                this.userRepository = userRepository;
                this.ensureMessagesLoaded = function () {
                    if (_this.isLoaded())
                        return;

                    _this.userRepository.getMessageSummaries(3, true, 40, function (messages) {
                        _this.unreadMessages(messages.receivedMessages);
                        _this.unreadMessagesCount(_this.unreadMessages().length);
                        _this.isLoaded(true);
                    });
                };
                this.isLoaded = ko.observable(false);
                this.reportCount = ko.observable(0);
                this.unreadMessages = ko.observableArray();
                this.entryType = ko.observable(entryType);
                this.searchTerm = ko.observable(searchTerm);
                this.unreadMessagesCount = ko.observable(unreadMessagesCount);

                this.entryTypeName = ko.computed(function () {
                    return entryTypeTranslations[_this.entryType()];
                });

                this.hasNotifications = ko.computed(function () {
                    return (_this.reportCount() > 0);
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
