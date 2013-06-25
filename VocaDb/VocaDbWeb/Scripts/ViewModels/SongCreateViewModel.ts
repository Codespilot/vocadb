/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/underscore/underscore.d.ts" />
/// <reference path="../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Repositories/SongRepository.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;
    declare var initEntrySearch: any;

    export class SongCreateViewModel {
        
        acceptArtistSelection: (artistId: number) => void;

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

        removeArtist: (artist: dc.ArtistContract) => void;

        constructor(songRepository: vdb.repositories.SongRepository) {

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
            
            if (this.pv1()) {
                this.checkDuplicatesAndPV();
            }

            this.acceptArtistSelection = (artistId: number) => {

                if (artistId) {
                    $.post("../../Artist/DataById", { id: artistId }, row => {
                        this.artists.push(row);
                    });
                }

            }

            this.removeArtist = (artist: dc.ArtistContract) => {
                this.artists.remove(artist);
            };

            var artistAddName = $("input#artistAddName");

            if (initEntrySearch) {
                initEntrySearch(artistAddName, null, "Artist", "../../Artist/FindJson",
                    {
                        allowCreateNew: false,
                        acceptSelection: this.acceptArtistSelection,
                        createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
                        createOptionSecondRow: function (item) { return item.AdditionalNames; },
                        extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer,Circle,OtherGroup,Unknown,Animator,Illustrator,Lyricist,OtherIndividual" },
                        height: 300
                    });
            }

        }
    
    }

}