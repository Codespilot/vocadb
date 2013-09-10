var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var sup = vdb.tests.testSupport;
            var vm = vdb.viewModels;
            

            var rep;
            var userRep = new sup.FakeUserRepository();
            var res = { createNewList: "Create new list" };
            var data = { id: 39, userRating: "Nothing" };

            var target;

            QUnit.module("SongDetailsViewModelTests", {
                setup: function () {
                    rep = new sup.FakeSongRepository();
                    rep.songLists = [{ id: 1, name: "Favorite Mikus" }];
                    target = new vm.SongDetailsViewModel(rep, userRep, res, data, null);
                }
            });

            test("constructor", function () {
                equal(target.id, 39, "id");
                ok(target.songListDialog, "songListDialog");
                ok(target.userRating, "userRating");
                equal(target.userRating.rating(), cls.SongVoteRating['Nothing'], "userRating.rating");
            });

            test("showSongLists has lists", function () {
                target.songListDialog.showSongLists();

                equal(target.songListDialog.songLists().length, 2, "songListDialog.songLists.length");
                equal(target.songListDialog.selectedListId(), 1, "songListDialog.selectedListId");
            });

            test("showSongLists no lists", function () {
                rep.songLists = [];
                target.songListDialog.showSongLists();

                equal(target.songListDialog.songLists().length, 1, "songListDialog.songLists.length");
                equal(target.songListDialog.selectedListId(), 0, "songListDialog.selectedListId");
            });

            test("addSongToList", function () {
                target.songListDialog.addSongToList();

                equal(rep.addedSongId, 39, "rep.addedSongId");
            });

            test("songInListsDialog show", function () {
                target.songInListsDialog.show();

                equal(target.songInListsDialog.dialogVisible(), true, "songInListsDialog.dialogVisible");
                ok(target.songInListsDialog.contentHtml(), "songInListsDialog.contentHtml");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
