/// <reference path="../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

    export class TopBarViewModel {
        
        public entryType: KnockoutObservable<string>;
        
        public searchTerm: KnockoutObservable<string>;

        public entryTypeName: KnockoutComputed<string>;

        constructor(entryTypeTranslations, entryType: string, searchTerm: string) {
            
            this.entryType = ko.observable(entryType);
            this.searchTerm = ko.observable(searchTerm);

            this.entryTypeName = ko.computed(() => {
                return entryTypeTranslations[this.entryType()];
            });
        
        }

    }

}