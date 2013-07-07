/// <reference path="../../Repositories/AlbumRepository.ts" />

module vdb.tests.testSupport {

    export class FakeAlbumRepository extends vdb.repositories.AlbumRepository {

        public deletedId: number;
        public updatedId: number;

        constructor() {

            super("");

            this.deleteArtistForAlbum = (artistForAlbumId, callback?) => {
                this.deletedId = artistForAlbumId;
                if (callback)
                    callback();
            }

            this.updateArtistForAlbumIsSupport = (artistForAlbumId, isSupport, callback?) => {
                this.updatedId = artistForAlbumId;
                if (callback)
                    callback();
            }

        }

    }

}