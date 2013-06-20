/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../ViewModels/PVRatingButtonsViewModel.ts" />
/// <reference path="../TestSupport/FakeUserRepository.ts" />

module vdb.tests.viewModels {

    import cls = vdb.models;
    import vm = vdb.viewModels;

    var repository = new vdb.tests.testSupport.FakeUserRepository();

    QUnit.module("PVRatingButtonsViewModel");

    test("constructor", () => {

        var target = new vm.PVRatingButtonsViewModel(repository, { Id: 39, Vote: cls.SongVoteRating[cls.SongVoteRating.Nothing] }, null);

        equal(cls.SongVoteRating.Nothing, target.rating(), "rating");

    });

}