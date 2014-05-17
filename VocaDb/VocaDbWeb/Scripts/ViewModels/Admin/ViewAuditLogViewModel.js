/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />
var vdb;
(function (vdb) {
    (function (viewModels) {
        var ViewAuditLogViewModel = (function () {
            function ViewAuditLogViewModel(data) {
                var _this = this;
                this.excludeUsers = ko.observable("");
                this.filter = ko.observable("");
                this.filterVisible = ko.observable(false);
                this.onlyNewUsers = ko.observable(false);
                this.toggleFilter = function () {
                    _this.filterVisible(!_this.filterVisible());
                };
                this.userName = ko.observable("");
                this.excludeUsers(data.excludeUsers);
                this.filter(data.filter);
                this.onlyNewUsers(data.onlyNewUsers);
                this.userName(data.userName);
                this.filterVisible(!vdb.functions.isNullOrWhiteSpace(data.userName) || !vdb.functions.isNullOrWhiteSpace(data.excludeUsers) || !vdb.functions.isNullOrWhiteSpace(data.filter) || data.onlyNewUsers);

                $("#userNameField").autocomplete({
                    source: vdb.functions.mapAbsoluteUrl("/User/FindByName"),
                    select: function (event, ui) {
                        _this.userName(ui.item.value);
                        return false;
                    }
                });

                $("#usersList").bind("keydown", function (event) {
                    if (event.keyCode === $.ui.keyCode.TAB && $(this).data("ui-autocomplete").menu.active) {
                        event.preventDefault();
                    }
                }).autocomplete({
                    minLength: 1,
                    source: function (request, response) {
                        var name = _this.extractLast(request.term);
                        if (name.length == 0)
                            response({});
                        else
                            $.get("/User/FindByName", { term: name, startsWith: true }, response);
                    },
                    focus: function () {
                        // prevent value inserted on focus
                        return false;
                    },
                    select: function (event, ui) {
                        var terms = _this.split($("#usersList").val());

                        // remove the current input
                        terms.pop();

                        // add the selected item
                        terms.push(ui.item.value);

                        // add placeholder to get the comma-and-space at the end
                        terms.push("");
                        _this.excludeUsers(terms.join(", "));
                        return false;
                    }
                });
            }
            ViewAuditLogViewModel.prototype.split = function (val) {
                return val.split(/,\s*/);
            };

            ViewAuditLogViewModel.prototype.extractLast = function (term) {
                return this.split(term).pop();
            };
            return ViewAuditLogViewModel;
        })();
        viewModels.ViewAuditLogViewModel = ViewAuditLogViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
