/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../DataContracts/User/UserMessageSummaryContract.ts" />
/// <reference path="../DataContracts/User/UserMessagesContract.ts" />
/// <reference path="../Repositories/EntryReportRepository.ts" />
/// <reference path="../Repositories/UserRepository.ts" />
var vdb;
(function (vdb) {
    (function (viewModels) {
        // View model for the top bar.
        var TopBarViewModel = (function () {
            // Initializes view model
            // entryTypeTranslations: translations for entry types.
            // entryType: currently selected entry type (for search).
            // unreadMessagesCount: number of unread received messages (includes notifications).
            // getNewReportsCount: whether to load new reports count (for mods only).
            // entryReportRepository: entry reports repository.
            // userRepository: user repository.
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
