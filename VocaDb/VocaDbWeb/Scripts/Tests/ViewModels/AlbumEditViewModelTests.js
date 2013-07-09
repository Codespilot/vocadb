var vdb;
(function (vdb) {
    (function (tests) {
        (function (viewModels) {
            var vm = vdb.viewModels;
            var dc = vdb.dataContracts;

            var rep = new vdb.tests.testSupport.FakeAlbumRepository();
            var songRep;
            var song;
            var categories = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
            var producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
            var vocalist = { id: 2, name: "Hatsune Miku", additionalNames: "", artistType: "Vocalist" };
            var producerArtistLink = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
            var vocalistArtistLink = { artist: vocalist, id: 40, isSupport: false, name: "", roles: "Default" };
            var songInAlbum;
            var roles = { Default: "Default", VoiceManipulator: "Voice manipulator" };
            var webLinkData = { category: "Official", description: "Youtube Channel", id: 123, url: "http://www.youtube.com/user/tripshots" };
            var data;

            QUnit.module("AlbumEditViewModelTests", {
                setup: function () {
                    songRep = new vdb.tests.testSupport.FakeSongRepository();
                    song = { additionalNames: "", artistString: "Tripshots", artists: [producer], id: 2, name: "Anger" };
                    songRep.song = song;

                    songInAlbum = {
                        artists: [producer],
                        artistString: "Tripshots",
                        discNumber: 1,
                        songAdditionalNames: "",
                        songId: 3,
                        songInAlbumId: 1,
                        songName: "Nebula",
                        trackNumber: 1
                    };

                    data = { artistLinks: [producerArtistLink, vocalistArtistLink], discType: "Album", tracks: [songInAlbum], webLinks: [webLinkData] };
                }
            });

            function createViewModel() {
                return new vm.AlbumEditViewModel(rep, songRep, roles, categories, data);
            }

            test("constructor", function () {
                var target = createViewModel();

                equal(target.artistLinks().length, 2, "artistLinks.length");
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

            test("acceptTrackSelection existing", function () {
                var target = createViewModel();
                target.tracks.removeAll();

                target.acceptTrackSelection(2, null);

                equal(target.tracks().length, 1, "tracks.length");
                equal(target.tracks()[0].songId, 2, "tracks[0].songId");
                equal(target.tracks()[0].songName, "Anger", "tracks[0].songName");
            });

            test("acceptTrackSelection new", function () {
                var target = createViewModel();
                target.tracks.removeAll();

                target.acceptTrackSelection(null, "Anger RMX");

                equal(target.tracks().length, 1, "tracks.length");
                equal(target.tracks()[0].songId, 0, "tracks[0].songId");
                equal(target.tracks()[0].songName, "Anger RMX", "tracks[0].songName");
            });

            test("acceptTrackSelection add a second track", function () {
                var target = createViewModel();

                target.acceptTrackSelection(2, null);

                equal(target.tracks().length, 2, "tracks.length");
                equal(target.tracks()[1].trackNumber(), 2, "tracks[1].trackNumber");
            });

            test("allTracksSelected", function () {
                var target = createViewModel();

                target.allTracksSelected(true);

                equal(target.tracks()[0].selected(), true, "tracks[0].selected");
            });

            test("editTrackProperties", function () {
                var target = createViewModel();
                var track = target.tracks()[0];

                target.editTrackProperties(track);

                ok(target.editedSong(), "editedSong");
                equal(target.editedSong().song, track, "editedSong.song");
                ok(target.editedSong().artistSelections, "editedSong.artistSelections");
                equal(target.editedSong().artistSelections.length, 2, "editedSong.artistSelections.length");
                equal(target.editedSong().artistSelections[0].artist, producer, "editedSong.artistSelections[0].artist");
                equal(target.editedSong().artistSelections[0].selected(), true, "editedSong.artistSelections[0].selected");
                equal(target.editedSong().artistSelections[1].artist, vocalist, "editedSong.artistSelections[1].artist");
                equal(target.editedSong().artistSelections[1].selected(), false, "editedSong.artistSelections[1].selected");
            });

            test("saveTrackProperties not changed", function () {
                var target = createViewModel();
                var track = target.tracks()[0];
                target.editTrackProperties(track);

                target.saveTrackProperties();

                equal(track.artists().length, 1, "track.artists.length");
                equal(track.artists()[0], producer, "track.artists[0]");
            });

            test("saveTrackProperties changed", function () {
                var target = createViewModel();
                var track = target.tracks()[0];
                target.editTrackProperties(track);
                target.editedSong().artistSelections[0].selected(false);

                target.saveTrackProperties();

                equal(track.artists().length, 0, "track.artists.length");
            });

            test("editMultipleTrackProperties", function () {
                var target = createViewModel();

                target.editMultipleTrackProperties();

                ok(target.editedSong(), "editedSong");
                ok(target.editedSong().artistSelections, "editedSong.artistSelections");
                equal(target.editedSong().artistSelections.length, 2, "editedSong.artistSelections.length");
                equal(target.editedSong().artistSelections[0].selected(), false, "editedSong.artistSelections[0].selected");
                equal(target.editedSong().artistSelections[1].selected(), false, "editedSong.artistSelections[1].selected");
            });

            test("addArtistsToSelectedTracks add new artists", function () {
                var target = createViewModel();
                var track = target.tracks()[0];
                track.selected(true);
                target.editMultipleTrackProperties();
                target.editedSong().artistSelections[1].selected(true);

                target.addArtistsToSelectedTracks();

                equal(track.artists().length, 2, "target.tracks[0].artists.length");
                equal(track.artists()[0], producer, "target.tracks[0].artists[0]");
                equal(track.artists()[1], vocalist, "target.tracks[0].artists[1]");
            });

            test("addArtistsToSelectedTracks not changed", function () {
                var target = createViewModel();
                var track = target.tracks()[0];
                track.selected(true);
                target.editMultipleTrackProperties();
                target.editedSong().artistSelections[0].selected(true);

                target.addArtistsToSelectedTracks();

                equal(track.artists().length, 1, "target.tracks[0].artists.length");
                equal(track.artists()[0], producer, "target.tracks[0].artists[0]");
            });

            test("removeArtistsFromSelectedTracks remove artist", function () {
                var target = createViewModel();
                var track = target.tracks()[0];
                track.selected(true);
                target.editMultipleTrackProperties();
                target.editedSong().artistSelections[0].selected(true);

                target.removeArtistsFromSelectedTracks();

                equal(track.artists().length, 0, "target.tracks[0].artists.length");
            });

            test("removeArtistsFromSelectedTracks not changed", function () {
                var target = createViewModel();
                var track = target.tracks()[0];
                track.selected(true);
                target.editMultipleTrackProperties();
                target.editedSong().artistSelections[1].selected(true);

                target.removeArtistsFromSelectedTracks();

                equal(track.artists().length, 1, "target.tracks[0].artists.length");
                equal(track.artists()[0], producer, "target.tracks[0].artists[0]");
            });

            test("getArtistLink found", function () {
                var target = createViewModel();

                var result = target.getArtistLink(39);

                ok(result, "result");
                equal(result.id, 39, "result.id");
            });

            test("getArtistLink not found", function () {
                var target = createViewModel();

                var result = target.getArtistLink(123);

                ok(!result, "result");
            });

            test("translateArtistRole", function () {
                var target = createViewModel();

                var result = target.translateArtistRole("VoiceManipulator");

                equal(result, "Voice manipulator", "result");
            });

            test("updateTrackNumbers updated by setting isNextDisc", function () {
                var target = createViewModel();
                target.acceptTrackSelection(2, null);
                target.tracks()[0].isNextDisc(true);

                equal(target.tracks()[0].discNumber(), 2, "tracks[0].discNumber");
                equal(target.tracks()[0].trackNumber(), 1, "tracks[0].trackNumber");
                equal(target.tracks()[1].trackNumber(), 2, "tracks[1].trackNumber");
            });
        })(tests.viewModels || (tests.viewModels = {}));
        var viewModels = tests.viewModels;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
