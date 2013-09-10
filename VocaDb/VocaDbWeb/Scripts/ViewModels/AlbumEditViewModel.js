var vdb;
(function (vdb) {
    (function (viewModels) {
        
        var rep = vdb.repositories;

        var AlbumEditViewModel = (function () {
            function AlbumEditViewModel(repository, songRepository, artistRoleNames, webLinkCategories, data) {
                var _this = this;
                this.repository = repository;
                this.acceptTrackSelection = function (songId, songName) {
                    if (songId) {
                        songRepository.getOne(songId, true, function (song) {
                            var track = new viewModels.SongInAlbumEditViewModel({ artists: song.artists, artistString: song.artistString, songAdditionalNames: song.additionalNames, songId: song.id, songName: song.name, discNumber: 1, songInAlbumId: 0, trackNumber: 1 });
                            track.isNextDisc.subscribe(function () {
                                return _this.updateTrackNumbers();
                            });
                            _this.tracks.push(track);
                        });
                    } else {
                        var track = new viewModels.SongInAlbumEditViewModel({ songName: songName, artists: [], artistString: "", discNumber: 1, songAdditionalNames: "", songId: 0, songInAlbumId: 0, trackNumber: 1 });
                        track.isNextDisc.subscribe(function () {
                            return _this.updateTrackNumbers();
                        });
                        _this.tracks.push(track);
                    }
                };

                this.addArtistsToSelectedTracks = function () {
                    _.forEach(_.filter(_this.tracks(), function (s) {
                        return s.selected();
                    }), function (song) {
                        var added = _.map(_.filter(_this.editedSong().artistSelections, function (a) {
                            return a.selected() && _.all(song.artists(), function (a2) {
                                return a.artist.id != a2.id;
                            });
                        }), function (a3) {
                            return a3.artist;
                        });
                        song.artists.push.apply(song.artists, added);
                    });

                    _this.trackPropertiesDialogVisible(false);
                };

                this.allTracksSelected = ko.observable(false);

                this.allTracksSelected.subscribe(function (selected) {
                    _.forEach(_this.tracks(), function (s) {
                        return s.selected(selected);
                    });
                });

                this.artistsForTracks = function () {
                    var notAllowedTypes = ['Label'];
                    return _.map(_.filter(_this.artistLinks(), function (a) {
                        return a.artist != null && !_.contains(notAllowedTypes, a.artist.artistType);
                    }), function (a) {
                        return a.artist;
                    });
                };

                this.artistLinks = ko.observableArray(_.map(data.artistLinks, function (artist) {
                    return new viewModels.ArtistForAlbumEditViewModel(repository, artist);
                }));

                this.editMultipleTrackProperties = function () {
                    var artists = _this.artistsForTracks();
                    _this.editedSong(new TrackPropertiesViewModel(artists, null));
                    _this.trackPropertiesDialogButtons([
                        { text: "Add to tracks", click: _this.addArtistsToSelectedTracks },
                        { text: "Remove from tracks", click: _this.removeArtistsFromSelectedTracks }
                    ]);
                    _this.trackPropertiesDialogVisible(true);
                };

                this.editTrackProperties = function (song) {
                    var artists = _this.artistsForTracks();
                    _this.editedSong(new TrackPropertiesViewModel(artists, song));
                    _this.trackPropertiesDialogButtons([{ text: 'Save', click: _this.saveTrackProperties }]);
                    _this.trackPropertiesDialogVisible(true);
                };

                this.editedSong = ko.observable(null);

                this.getArtistLink = function (artistForAlbumId) {
                    return _.find(_this.artistLinks(), function (artist) {
                        return artist.id == artistForAlbumId;
                    });
                };

                this.removeArtist = function (artistForAlbum) {
                    _this.artistLinks.remove(artistForAlbum);
                    repository.deleteArtistForAlbum(artistForAlbum.id);
                };

                this.removeArtistsFromSelectedTracks = function () {
                    _.forEach(_.filter(_this.tracks(), function (s) {
                        return s.selected();
                    }), function (song) {
                        var removed = _.filter(song.artists(), function (a) {
                            return _.some(_this.editedSong().artistSelections, function (a2) {
                                return a2.selected() && a.id == a2.artist.id;
                            });
                        });
                        song.artists.removeAll(removed);
                    });

                    _this.trackPropertiesDialogVisible(false);
                };

                this.removeTrack = function (song) {
                    _this.tracks.remove(song);
                };

                this.saveTrackProperties = function () {
                    _this.trackPropertiesDialogVisible(false);

                    if (_this.editedSong) {
                        var selected = _.map(_.filter(_this.editedSong().artistSelections, function (a) {
                            return a.selected();
                        }), function (a) {
                            return a.artist;
                        });
                        _this.editedSong().song.artists(selected);
                        _this.editedSong(null);
                    }
                };

                this.trackPropertiesDialogButtons = ko.observableArray([{ text: 'Save', click: this.saveTrackProperties }]);

                this.trackPropertiesDialogVisible = ko.observable(false);

                this.tracks = ko.observableArray(_.map(data.tracks, function (song) {
                    return new viewModels.SongInAlbumEditViewModel(song);
                }));

                _.forEach(this.tracks(), function (song) {
                    song.isNextDisc.subscribe(function () {
                        return _this.updateTrackNumbers();
                    });
                });

                this.tracks.subscribe(function () {
                    return _this.updateTrackNumbers();
                });

                var songTypes = "Unspecified,Original,Remaster,Remix,Cover,Mashup,Other,Instrumental";

                if (data.discType == "Video")
                    songTypes += ",MusicPV,DramaPV";

                this.trackSearchParams = {
                    acceptSelection: this.acceptTrackSelection,
                    createNewItem: "Create new song named '{0}'.",
                    extraQueryParams: { songTypes: songTypes }
                };

                this.translateArtistRole = function (role) {
                    return artistRoleNames[role];
                };

                this.updateTrackNumbers = function () {
                    var track = 1;
                    var disc = 1;

                    _.forEach(_this.tracks(), function (song) {
                        if (song.isNextDisc()) {
                            disc++;
                            track = 1;
                        }

                        song.discNumber(disc);
                        song.trackNumber(track);
                        track++;
                    });
                };

                this.webLinks = new viewModels.WebLinksEditViewModel(data.webLinks, webLinkCategories);
            }
            return AlbumEditViewModel;
        })();
        viewModels.AlbumEditViewModel = AlbumEditViewModel;

        var TrackArtistSelectionViewModel = (function () {
            function TrackArtistSelectionViewModel(artist, selected, filter) {
                this.artist = artist;
                this.selected = ko.observable(selected);

                this.visible = ko.computed(function () {
                    var f = filter();
                    if (f.length == 0)
                        return true;

                    f = f.trim().toLowerCase();

                    return (artist.name.toLowerCase().indexOf(f) >= 0 || artist.additionalNames.toLowerCase().indexOf(f) >= 0);
                });
            }
            return TrackArtistSelectionViewModel;
        })();
        viewModels.TrackArtistSelectionViewModel = TrackArtistSelectionViewModel;

        var TrackPropertiesViewModel = (function () {
            function TrackPropertiesViewModel(artists, song) {
                var _this = this;
                this.song = song;
                this.filter = ko.observable("");
                this.artistSelections = _.map(artists, function (a) {
                    return new TrackArtistSelectionViewModel(a, song != null && _.some(song.artists(), function (sa) {
                        return a.id == sa.id;
                    }), _this.filter);
                });
            }
            return TrackPropertiesViewModel;
        })();
        viewModels.TrackPropertiesViewModel = TrackPropertiesViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
