/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

module vdb.viewModels {

    export class ViewAuditLogViewModel {

        public excludeUsers = ko.observable("");

        public filter = ko.observable("");

        public filterVisible = ko.observable(false);

        public onlyNewUsers = ko.observable(false);

        public toggleFilter = () => {
            this.filterVisible(!this.filterVisible());
        }

        private split(val: string) {
            return val.split(/,\s*/);
        }

        private extractLast(term: string) {
            return this.split(term).pop();
        }

        constructor(data: ViewAuditLogContract) {

            this.excludeUsers(data.excludeUsers);
            this.filter(data.filter);
            this.onlyNewUsers(data.onlyNewUsers);
            this.filterVisible(!vdb.functions.isNullOrWhiteSpace(data.excludeUsers)
                || !vdb.functions.isNullOrWhiteSpace(data.filter)
                || data.onlyNewUsers);
            
            $("#usersList")
            // don't navigate away from the field on tab when selecting an item
                .bind("keydown", function (event) {
                    if (event.keyCode === $.ui.keyCode.TAB &&
                        $(this).data("ui-autocomplete").menu.active) {
                        event.preventDefault();
                    }
                })
                .autocomplete({
                    minLength: 1,
                    source: (request, response) => {
                        var name = this.extractLast(request.term);
                        if (name.length == 0)
                            response({});
                        else
                            $.get("/User/FindByName", { term: name, startsWith: true }, response);
                    },
                    focus: function () {
                        // prevent value inserted on focus
                        return false;
                    },
                    select: (event, ui) => {
                        var terms = this.split($("#usersList").val());
                        // remove the current input
                        terms.pop();
                        // add the selected item
                        terms.push(ui.item.value);
                        // add placeholder to get the comma-and-space at the end
                        terms.push("");
                        this.excludeUsers(terms.join(", "));
                        return false;
                    }
                });

        }
    
    }

    export interface ViewAuditLogContract {

        excludeUsers: string;

        filter: string;

        onlyNewUsers: boolean;

    }

}