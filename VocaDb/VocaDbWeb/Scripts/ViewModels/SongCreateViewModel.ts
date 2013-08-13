/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../KnockoutExtensions/ArtistAutoComplete.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Repositories/SongRepository.ts" />
/// <reference path="../Repositories/ArtistRepository.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;

    // View model for song creation view
    export class SongCreateViewModel {
        
        addArtist: (artistId: number) => void;

        artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
        
        artists: KnockoutObservableArray<dc.ArtistContract> = ko.observableArray([]);

        checkDuplicatesAndPV: () => void;

        dupeEntries: KnockoutObservableArray<dc.DuplicateEntryResultContract> = ko.observableArray([]);

        isDuplicatePV: KnockoutComputed<boolean>;

        nameOriginal = ko.observable("");
        nameRomaji = ko.observable("");
        nameEnglish = ko.observable("");
        pv1 = ko.observable("");
        pv2 = ko.observable("");

        hasName: KnockoutComputed<boolean>;

        public submit = () => {
            this.submitting(true);
            return true;
        }

        public submitting = ko.observable(false);

        removeArtist: (artist: dc.ArtistContract) => void;

        constructor(songRepository: vdb.repositories.SongRepository, artistRepository: vdb.repositories.ArtistRepository, data?) {

            if (data) {
                this.nameOriginal(data.nameOriginal || "");
                this.nameRomaji(data.nameRomaji || "");
                this.nameEnglish(data.nameEnglish || "");
                this.pv1(data.pvUrl || "");
                this.pv2(data.reprintPVUrl || "");
                this.artists(data.artists || []);
            }

            this.addArtist = (artistId: number) => {

                if (artistId) {
                    artistRepository.getOne(artistId, artist => {
                        this.artists.push(artist);
                    });
                }

            }

            this.artistSearchParams = {
                allowCreateNew: false,
                acceptSelection: this.addArtist,
                extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,OtherVoiceSynthesizer,Producer,Circle,OtherGroup,Unknown,Animator,Illustrator,Lyricist,OtherIndividual" },
                height: 300
            };

            this.hasName = ko.computed(() => {
                return this.nameOriginal().length > 0 || this.nameRomaji().length > 0 || this.nameEnglish().length > 0;
            });

            this.isDuplicatePV = ko.computed(() => {
                return _.some(this.dupeEntries(), item => { return item.matchProperty == 'PV' });
            });
            
            this.checkDuplicatesAndPV = () => {
                
                var term1 = this.nameOriginal();
                var term2 = this.nameRomaji();
                var term3 = this.nameEnglish();
                var pv1 = this.pv1();
                var pv2 = this.pv2();

                songRepository.findDuplicate(
                    { term1: term1, term2: term2, term3: term3, pv1: pv1, pv2: pv2, getPVInfo: true },
                    result => {

                    this.dupeEntries(result.matches);

                    if (result.title && !this.hasName()) {
                        this.nameOriginal(result.title);
                    }

                    if (result.artists && this.artists().length == 0) {

                        _.forEach(result.artists, artist => {
                            this.artists.push(artist);
                        });

                    }

                });

            }

            this.removeArtist = (artist: dc.ArtistContract) => {
                this.artists.remove(artist);
            };
            
            if (this.pv1()) {
                this.checkDuplicatesAndPV();
            }

        }
    
    }

}