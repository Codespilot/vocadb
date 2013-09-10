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
                this.excludeUsers(data.excludeUsers);
                this.filter(data.filter);
                this.onlyNewUsers(data.onlyNewUsers);
                this.filterVisible(!vdb.functions.isNullOrWhiteSpace(data.excludeUsers) || !vdb.functions.isNullOrWhiteSpace(data.filter) || data.onlyNewUsers);

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
                        return false;
                    },
                    select: function (event, ui) {
                        var terms = _this.split($("#usersList").val());

                        terms.pop();

                        terms.push(ui.item.value);

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
