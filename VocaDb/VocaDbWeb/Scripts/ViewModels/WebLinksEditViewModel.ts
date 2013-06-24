/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="WebLinkEditViewModel.ts" />

module vdb.viewModels {

    export class WebLinksEditViewModel {

        public add: () => void;

        public remove: (webLink) => void;

        public webLinks: KnockoutObservableArray<WebLinkEditViewModel>;

        constructor(webLinkContracts: vdb.dataContracts.WebLinkContract[]) {
            
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

