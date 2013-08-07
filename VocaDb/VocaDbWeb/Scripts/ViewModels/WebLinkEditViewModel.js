var vdb;
(function (vdb) {
    /// <reference path="../typings/knockout/knockout.d.ts" />
    /// <reference path="../DataContracts/WebLinkContract.ts" />
    /// <reference path="../Models/WebLinkCategory.ts" />
    /// <reference path="../Shared/WebLinkMatcher.ts" />
    (function (viewModels) {
        var cls = vdb.models;

        var WebLinkEditViewModel = (function () {
            function WebLinkEditViewModel(data) {
                var _this = this;
                if (data) {
                    this.category = ko.observable(data.category);
                    this.description = ko.observable(data.description);
                    this.id = data.id;
                    this.url = ko.observable(data.url);
                } else {
                    this.category = ko.observable(cls.WebLinkCategory[cls.WebLinkCategory.Other]);
                    this.description = ko.observable("");
                    this.id = 0;
                    this.url = ko.observable("");
                }

                this.url.subscribe(function (url) {
                    if (!_this.description()) {
                        var matcher = vdb.utils.WebLinkMatcher.matchWebLink(url);

                        if (matcher) {
                            _this.description(matcher.desc);
                            _this.category(cls.WebLinkCategory[matcher.cat]);
                        }
                    }
                });
            }
            return WebLinkEditViewModel;
        })();
        viewModels.WebLinkEditViewModel = WebLinkEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
