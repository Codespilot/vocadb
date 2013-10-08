/// <reference path="../../Repositories/UserRepository.ts" />

module vdb.tests.testSupport {

    import cls = vdb.models;

    export class FakeUserRepository extends vdb.repositories.UserRepository {
        
        public songId: number;
        public rating: cls.SongVoteRating;

        constructor() {

            super(new vdb.UrlMapper(""));        

            this.updateSongRating = (songId: number, rating: cls.SongVoteRating, callback: Function) => {

                this.songId = songId;
                this.rating = rating;

                if (callback)
                    callback();

            };

        }
    
    }

}