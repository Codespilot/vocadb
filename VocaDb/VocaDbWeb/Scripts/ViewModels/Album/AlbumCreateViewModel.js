/// <reference path="../../typings/knockout/knockout.d.ts" />
var vdb;
(function (vdb) {
    (function (viewModels) {
        var AlbumCreateViewModel = (function () {
            function AlbumCreateViewModel() {
                var _this = this;
                this.submit = function () {
                    _this.submitting(true);
                    return true;
                };
                this.submitting = ko.observable(false);
            }
            return AlbumCreateViewModel;
        })();
        viewModels.AlbumCreateViewModel = AlbumCreateViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
