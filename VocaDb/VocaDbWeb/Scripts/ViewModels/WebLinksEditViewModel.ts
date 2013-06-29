/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinkEditViewModel.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;

    export class WebLinksEditViewModel {

        public add: () => void;

        public remove: (webLink) => void;

        public webLinks: KnockoutObservableArray<WebLinkEditViewModel>;

        constructor(webLinkContracts: dc.WebLinkContract[], public categories?: dc.TranslatedEnumField[]) {
            
            this.webLinks = ko.observableArray(_.map(webLinkContracts, contract => new WebLinkEditViewModel(contract)));
            
            this.add = () => {
                this.webLinks.push(new WebLinkEditViewModel());
            };

            this.remove = (webLink) => {
                this.webLinks.remove(webLink);
            }

        }

    }

}

