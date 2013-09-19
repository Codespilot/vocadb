/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Repositories/EntryReportRepository.ts" />

module vdb.viewModels {

    export class TopBarViewModel {
        
        public entryType: KnockoutObservable<string>;

        public hasNotifications: KnockoutComputed<boolean>;

        public reportCount = ko.observable(0);

        public searchTerm: KnockoutObservable<string>;

        public entryTypeName: KnockoutComputed<string>;

        constructor(entryTypeTranslations, entryType: string, searchTerm: string, hasUnreadMessages: boolean,
            getNewReportsCount: boolean, entryReportRepository: vdb.repositories.EntryReportRepository) {
            
            this.entryType = ko.observable(entryType);
            this.searchTerm = ko.observable(searchTerm);

            this.entryTypeName = ko.computed(() => {
                return entryTypeTranslations[this.entryType()];
            });

            this.hasNotifications = ko.computed(() => {
                return (hasUnreadMessages || this.reportCount() > 0);
            });

            if (getNewReportsCount) {
                entryReportRepository.getNewReportCount(count => {
                    this.reportCount(count);
                });
            }
        
        }

    }

}