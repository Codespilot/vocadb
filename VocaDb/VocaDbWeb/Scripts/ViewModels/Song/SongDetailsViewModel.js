var vdb;
(function (vdb) {
    (function (viewModels) {
        var dc = vdb.dataContracts;
        var rep = vdb.repositories;

        var SongDetailsViewModel = (function () {
            function SongDetailsViewModel(repository, userRepository, resources, data, ratingCallback) {
                var _this = this;
                this.id = data.id;
                this.userRating = new viewModels.PVRatingButtonsViewModel(userRepository, { id: data.id, vote: data.userRating }, ratingCallback);

                this.allVersionsVisible = ko.observable(false);

                this.getUsers = function () {
                    repository.usersWithSongRating(_this.id, function (result) {
                        _this.usersContent(result);
                        _this.usersPopupVisible(true);
                    });
                };

                this.showAllVersions = function () {
                    _this.allVersionsVisible(true);
                };

                this.songInListsDialog = new SongInListsViewModel(repository, this.id);
                this.songListDialog = new SongListsViewModel(repository, resources, this.id);

                this.usersContent = ko.observable();

                this.usersPopupVisible = ko.observable(false);
            }
            return SongDetailsViewModel;
        })();
        viewModels.SongDetailsViewModel = SongDetailsViewModel;

        var SongInListsViewModel = (function () {
            function SongInListsViewModel(repository, songId) {
                var _this = this;
                this.contentHtml = ko.observable();
                this.dialogVisible = ko.observable(false);
                this.show = function () {
                    repository.songListsForSong(songId, function (result) {
                        _this.contentHtml(result);
                        _this.dialogVisible(true);
                    });
                };
            }
            return SongInListsViewModel;
        })();
        viewModels.SongInListsViewModel = SongInListsViewModel;

        var SongListsInCategory = (function () {
            function SongListsInCategory(categoryName, songLists) {
                this.categoryName = categoryName;
                this.songLists = songLists;
            }
            return SongListsInCategory;
        })();
        viewModels.SongListsInCategory = SongListsInCategory;

        var SongListsViewModel = (function () {
            function SongListsViewModel(repository, resources, songId) {
                var _this = this;
                this.dialogVisible = ko.observable(false);
                this.newListName = ko.observable("");
                this.selectedListId = ko.observable(null);
                this.songLists = ko.observableArray();
                var isValid = function () {
                    return (_this.selectedListId() != 0 || _this.newListName().length > 0);
                };

                this.addSongToList = function () {
                    if (isValid())
                        repository.addSongToList(_this.selectedListId(), songId, _this.newListName(), function () {
                            _this.dialogVisible(false);
                            if (_this.addedToList)
                                _this.addedToList();
                        });
                };

                this.showSongLists = function () {
                    repository.songListsForUser(songId, function (songLists) {
                        songLists.push({ id: 0, name: resources.createNewList });
                        _this.songLists(songLists);
                        _this.newListName("");
                        _this.selectedListId(songLists.length > 0 ? songLists[0].id : undefined);
                        _this.dialogVisible(true);
                    });
                };
            }
            return SongListsViewModel;
        })();
        viewModels.SongListsViewModel = SongListsViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
