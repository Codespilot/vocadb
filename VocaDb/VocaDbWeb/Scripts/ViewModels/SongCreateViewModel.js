var vdb;
(function (vdb) {
    (function (viewModels) {
        var SongCreateViewModel = (function () {
            function SongCreateViewModel(songRepository, artistRepository, data) {
                var _this = this;
                this.artists = ko.observableArray([]);
                this.dupeEntries = ko.observableArray([]);
                this.nameOriginal = ko.observable("");
                this.nameRomaji = ko.observable("");
                this.nameEnglish = ko.observable("");
                this.pv1 = ko.observable("");
                this.pv2 = ko.observable("");
                this.songType = ko.observable("Original");
                this.submit = function () {
                    _this.submitting(true);
                    return true;
                };
                this.submitting = ko.observable(false);
                if (data) {
                    this.nameOriginal(data.nameOriginal || "");
                    this.nameRomaji(data.nameRomaji || "");
                    this.nameEnglish(data.nameEnglish || "");
                    this.pv1(data.pvUrl || "");
                    this.pv2(data.reprintPVUrl || "");
                    this.artists(data.artists || []);
                }

                this.addArtist = function (artistId) {
                    if (artistId) {
                        artistRepository.getOne(artistId, function (artist) {
                            _this.artists.push(artist);
                        });
                    }
                };

                this.artistSearchParams = {
                    allowCreateNew: false,
                    acceptSelection: this.addArtist,
                    extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,OtherVoiceSynthesizer,Producer,Circle,OtherGroup,Unknown,Animator,Illustrator,Lyricist,OtherIndividual" },
                    height: 300
                };

                this.hasName = ko.computed(function () {
                    return _this.nameOriginal().length > 0 || _this.nameRomaji().length > 0 || _this.nameEnglish().length > 0;
                });

                this.isDuplicatePV = ko.computed(function () {
                    return _.some(_this.dupeEntries(), function (item) {
                        return item.matchProperty == 'PV';
                    });
                });

                this.checkDuplicatesAndPV = function () {
                    var term1 = _this.nameOriginal();
                    var term2 = _this.nameRomaji();
                    var term3 = _this.nameEnglish();
                    var pv1 = _this.pv1();
                    var pv2 = _this.pv2();

                    songRepository.findDuplicate({ term1: term1, term2: term2, term3: term3, pv1: pv1, pv2: pv2, getPVInfo: true }, function (result) {
                        _this.dupeEntries(result.matches);

                        if (result.title && !_this.hasName()) {
                            _this.nameOriginal(result.title);
                        }

                        if (result.songType && result.songType != "Unspecified") {
                            _this.songType(result.songType);
                        }

                        if (result.artists && _this.artists().length == 0) {
                            _.forEach(result.artists, function (artist) {
                                _this.artists.push(artist);
                            });
                        }
                    });
                };

                this.removeArtist = function (artist) {
                    _this.artists.remove(artist);
                };

                if (this.pv1()) {
                    this.checkDuplicatesAndPV();
                }
            }
            return SongCreateViewModel;
        })();
        viewModels.SongCreateViewModel = SongCreateViewModel;
    })(vdb.viewModels || (vdb.viewModels = {}));
    var viewModels = vdb.viewModels;
})(vdb || (vdb = {}));
