/// <reference path="../../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />

module vdb.tests.testSupport {

    import dc = vdb.dataContracts;

    export class FakeSongRepository extends vdb.repositories.SongRepository {

        results: dc.NewSongCheckResultContract = null;
        song: dc.SongWithComponentsContract = null;

        constructor() {
            
            super("");

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {
                if (callback)
                    callback(this.results);
            };

            this.getOne = (id, includeArtists = false, callback?) => {
                if (callback)
                    callback(this.song);
            }
        
        }

    }

}