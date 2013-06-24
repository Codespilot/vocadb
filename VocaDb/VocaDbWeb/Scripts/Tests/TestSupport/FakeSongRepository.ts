/// <reference path="../../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />

module vdb.tests.testSupport {

    import dc = vdb.dataContracts;

    export class FakeSongRepository extends vdb.repositories.SongRepository {

        results: dc.NewSongCheckResultContract = null;

        constructor() {
            
            super("");

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {
                if (callback)
                    callback(this.results);
            };
        
        }

    }

}