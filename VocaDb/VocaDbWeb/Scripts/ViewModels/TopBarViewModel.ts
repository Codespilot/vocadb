/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../DataContracts/User/UserMessageSummaryContract.ts" />
/// <reference path="../DataContracts/User/UserMessagesContract.ts" />
/// <reference path="../Repositories/EntryReportRepository.ts" />
/// <reference path="../Repositories/UserRepository.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;

    export class TopBarViewModel {

        public ensureMessagesLoaded = () => {

            if (this.unreadMessages.length > 0)
                return;

            this.userRepository.getMessageSummaries(3, true, 40, (messages: dc.UserMessagesContract) => {
                this.unreadMessages(messages.receivedMessages);
            });

            this.isLoaded(true);

        };

        public entryType: KnockoutObservable<string>;

        public hasNotifications: KnockoutComputed<boolean>;

        public isLoaded = ko.observable(false);

        public reportCount = ko.observable(0);

        public searchTerm: KnockoutObservable<string>;

        public entryTypeName: KnockoutComputed<string>;

        public unreadMessages: KnockoutObservableArray<dc.UserMessageSummaryContract> = ko.observableArray();

        public unreadMessagesCount: KnockoutObservable<number>;

        constructor(entryTypeTranslations, entryType: string, searchTerm: string, unreadMessagesCount: number,
            getNewReportsCount: boolean, entryReportRepository: vdb.repositories.EntryReportRepository, private userRepository: vdb.repositories.UserRepository) {
            
            this.entryType = ko.observable(entryType);
            this.searchTerm = ko.observable(searchTerm);
            this.unreadMessagesCount = ko.observable(unreadMessagesCount);

            this.entryTypeName = ko.computed(() => {
                return entryTypeTranslations[this.entryType()];
            });

            this.hasNotifications = ko.computed(() => {
                return (this.reportCount() > 0);
            });

            if (getNewReportsCount) {
                entryReportRepository.getNewReportCount(count => {
                    this.reportCount(count);
                });
            }
        
        }

    }

}