/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../TestSupport/FakeAlbumRepository.ts" />
/// <reference path="../TestSupport/FakeSongRepository.ts" />
/// <reference path="../../ViewModels/AlbumEditViewModel.ts" />

module vdb.tests.viewModels {

    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var rep = new vdb.tests.testSupport.FakeAlbumRepository();
    var songRep: vdb.tests.testSupport.FakeSongRepository;

    var song: dc.SongWithComponentsContract;
    var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];

    var producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
    var vocalist = { id: 2, name: "Hatsune Miku", additionalNames: "", artistType: "Vocalist" };
    var label = { id: 3, name: "KarenT", additionalNames: "", artistType: "Label" };

    var producerArtistLink = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
    var vocalistArtistLink = { artist: vocalist, id: 40, isSupport: false, name: "", roles: "Default" };
    var labelArtistLink = { artist: label, id: 41, isSupport: false, name: "", roles: "Default" };
    var customArtistLink = { artist: null, id: 42, isSupport: false, name: "xxJulexx", roles: "Default" };

    var songInAlbum: vm.SongInAlbumEditContract;
    var roles = { Default: "Default", VoiceManipulator: "Voice manipulator" };
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 123, url: "http://www.youtube.com/user/tripshots" };
    var data: vm.AlbumEdit;

    QUnit.module("AlbumEditViewModelTests", {
        setup: () => {

            songRep = new vdb.tests.testSupport.FakeSongRepository();
            song = { additionalNames: "", artistString: "Tripshots", artists: [producer], id: 2, name: "Anger" };
            songRep.song = song;

            songInAlbum = {
                artists: [producer], artistString: "Tripshots", discNumber: 1, songAdditionalNames: "",
                songId: 3, songInAlbumId: 1, songName: "Nebula", trackNumber: 1
            };

            data = { artistLinks: [producerArtistLink, vocalistArtistLink, labelArtistLink, customArtistLink], discType: "Album", tracks: [songInAlbum], webLinks: [webLinkData] };

        }
    });

    function createViewModel() {
        return new vm.AlbumEditViewModel(rep, songRep, roles, categories, data);
    }

    test("constructor", () => {

        var target = createViewModel();

        equal(target.artistLinks().length, 4, "artistLinks.length");
        equal(target.artistLinks()[0].id, 39, "artistLinks[0].id");
        ok(target.artistLinks()[0].artist, "artistLinks[0].artist");
        equal(target.artistLinks()[0].artist, producer, "artistLinks[0].artist");
        equal(target.tracks().length, 1, "tracks.length");
        equal(target.tracks()[0].songId, 3, "tracks[0].songId");
        equal(target.tracks()[0].songName, "Nebula", "tracks[0].songName");
        equal(target.tracks()[0].selected(), false, "tracks[0].selected");
        equal(target.tracks()[0].trackNumber(), 1, "tracks[0].trackNumber");
        equal(target.webLinks.webLinks().length, 1, "webLinks.length");
        equal(target.webLinks.webLinks()[0].id, 123, "webLinks[0].id");

    });

    test("acceptTrackSelection existing", () => {

        var target = createViewModel();
        target.tracks.removeAll();

        target.acceptTrackSelection(2, null);

        equal(target.tracks().length, 1, "tracks.length");
        equal(target.tracks()[0].songId, 2, "tracks[0].songId");
        equal(target.tracks()[0].songName, "Anger", "tracks[0].songName");

    });

    test("acceptTrackSelection new", () => {

        var target = createViewModel();
        target.tracks.removeAll();

        target.acceptTrackSelection(null, "Anger RMX");

        equal(target.tracks().length, 1, "tracks.length");
        equal(target.tracks()[0].songId, 0, "tracks[0].songId");
        equal(target.tracks()[0].songName, "Anger RMX", "tracks[0].songName");

    });

    test("acceptTrackSelection add a second track", () => {

        var target = createViewModel();

        target.acceptTrackSelection(2, null);

        equal(target.tracks().length, 2, "tracks.length");
        equal(target.tracks()[1].trackNumber(), 2, "tracks[1].trackNumber");

    });

    test("allTracksSelected", () => {

        var target = createViewModel();

        target.allTracksSelected(true);

        equal(target.tracks()[0].selected(), true, "tracks[0].selected");

    });

    test("editTrackProperties", () => {

        var target = createViewModel();
        var track = target.tracks()[0];

        target.editTrackProperties(track);

        ok(target.editedSong(), "editedSong");
        equal(target.editedSong().song, track, "editedSong.song");
        ok(target.editedSong().artistSelections, "editedSong.artistSelections");
        equal(target.editedSong().artistSelections.length, 2, "editedSong.artistSelections.length");
        equal(target.editedSong().artistSelections[0].artist, producer, "editedSong.artistSelections[0].artist");
        equal(target.editedSong().artistSelections[0].selected(), true, "editedSong.artistSelections[0].selected");   // Selected, because added to song
        equal(target.editedSong().artistSelections[1].artist, vocalist, "editedSong.artistSelections[1].artist");
        equal(target.editedSong().artistSelections[1].selected(), false, "editedSong.artistSelections[1].selected");  // Not seleted, because not added yet

    });

    test("saveTrackProperties not changed", () => {

        var target = createViewModel();
        var track = target.tracks()[0];
        target.editTrackProperties(track);

        target.saveTrackProperties();

        equal(track.artists().length, 1, "track.artists.length");
        equal(track.artists()[0], producer, "track.artists[0]");

    });

    test("saveTrackProperties changed", () => {

        var target = createViewModel();
        var track = target.tracks()[0];
        target.editTrackProperties(track);
        target.editedSong().artistSelections[0].selected(false);

        target.saveTrackProperties();

        equal(track.artists().length, 0, "track.artists.length");

    });

    test("editMultipleTrackProperties", () => {

        var target = createViewModel();

        target.editMultipleTrackProperties();

        ok(target.editedSong(), "editedSong");
        ok(target.editedSong().artistSelections, "editedSong.artistSelections");
        equal(target.editedSong().artistSelections.length, 2, "editedSong.artistSelections.length");
        equal(target.editedSong().artistSelections[0].selected(), false, "editedSong.artistSelections[0].selected");
        equal(target.editedSong().artistSelections[1].selected(), false, "editedSong.artistSelections[1].selected");

    });

    // Add an artist to a track that was not added before
    test("addArtistsToSelectedTracks add new artists", () => {

        var target = createViewModel();
        var track = target.tracks()[0];
        track.selected(true);
        target.editMultipleTrackProperties();
        target.editedSong().artistSelections[1].selected(true); // Select vocalist, which is not added yet

        target.addArtistsToSelectedTracks();

        equal(track.artists().length, 2, "target.tracks[0].artists.length");
        equal(track.artists()[0], producer, "target.tracks[0].artists[0]");
        equal(track.artists()[1], vocalist, "target.tracks[0].artists[1]");

    });

    test("addArtistsToSelectedTracks not changed", () => {

        var target = createViewModel();
        var track = target.tracks()[0];
        track.selected(true);
        target.editMultipleTrackProperties();
        target.editedSong().artistSelections[0].selected(true); // Select producer, who is added already

        target.addArtistsToSelectedTracks();

        equal(track.artists().length, 1, "target.tracks[0].artists.length");
        equal(track.artists()[0], producer, "target.tracks[0].artists[0]");

    });

    test("removeArtistsFromSelectedTracks remove artist", () => {

        var target = createViewModel();
        var track = target.tracks()[0];
        track.selected(true);
        target.editMultipleTrackProperties();
        target.editedSong().artistSelections[0].selected(true); // Select producer, who is added already

        target.removeArtistsFromSelectedTracks();

        equal(track.artists().length, 0, "target.tracks[0].artists.length");

    });

    test("removeArtistsFromSelectedTracks not changed", () => {

        var target = createViewModel();
        var track = target.tracks()[0];
        track.selected(true);
        target.editMultipleTrackProperties();
        target.editedSong().artistSelections[1].selected(true); // Select vocalist, who isn't added

        target.removeArtistsFromSelectedTracks();

        equal(track.artists().length, 1, "target.tracks[0].artists.length");
        equal(track.artists()[0], producer, "target.tracks[0].artists[0]");

    });

    test("getArtistLink found", () => {

        var target = createViewModel();

        var result = target.getArtistLink(39);

        ok(result, "result");
        equal(result.id, 39, "result.id");

    });

    test("getArtistLink not found", () => {

        var target = createViewModel();

        var result = target.getArtistLink(123);

        ok(!result, "result");

    });

    test("translateArtistRole", () => {

        var target = createViewModel();

        var result = target.translateArtistRole("VoiceManipulator");

        equal(result, "Voice manipulator", "result");

    });

    test("updateTrackNumbers updated by setting isNextDisc", () => {

        var target = createViewModel();
        target.acceptTrackSelection(2, null);
        target.tracks()[0].isNextDisc(true);

        equal(target.tracks()[0].discNumber(), 2, "tracks[0].discNumber");
        equal(target.tracks()[0].trackNumber(), 1, "tracks[0].trackNumber");
        equal(target.tracks()[1].trackNumber(), 2, "tracks[1].trackNumber");

    });

}