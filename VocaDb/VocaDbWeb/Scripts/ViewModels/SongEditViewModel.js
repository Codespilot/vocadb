var vdb;
(function (vdb) {
    (function (viewModels) {
        var SongEditViewModel = (function () {
            function SongEditViewModel(webLinkCategories, data) {
                var _this = this;
                this.submitting = ko.observable(false);
                this.submit = function () {
                    _this.submitting(true);
                    return true;
                };
                this.length = ko.observable(data.length);
                this.webLinks = new vdb.viewModels.WebLinksEditViewModel(data.webLinks, webLinkCategories);

                this.lengthFormatted = ko.computed({
                    read: function () {
                        var mins = Math.floor(_this.length() / 60);
                        return mins + ":" + _this.addLeadingZero(_this.length() % 60);
                    },
                    write: function (value) {
                        var parts = value.split(":");
                        if (parts.length == 2 && parseInt(parts[0], 10) != NaN && parseInt(parts[1], 10) != NaN) {
                            _this.length(parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10));
                        } else if (parts.length == 1 && !isNaN(parseInt(parts[0], 10))) {
                            _this.length(parseInt(parts[0], 10));
                        } else {
                            _this.length(0);
                        }
                    }
                });
            }
            SongEditViewModel.prototype.addLeadingZero = function (val) {
                return (val < 10 ? "0" + val : val);
            };
            return SongEditViewModel;
        })();
        viewModels.SongEditViewModel = SongEditViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
