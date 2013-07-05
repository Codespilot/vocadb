/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../DataContracts/ArtistContract.ts" />
/// <reference path="../DataContracts/DuplicateEntryResultContract.ts" />

module vdb.repositories {

    import dc = vdb.dataContracts;

    // Repository for managing artists and related objects.
    // Corresponds to the ArtistController class.
    export class ArtistRepository {

        public findDuplicate: (params, callback: (result: dc.DuplicateEntryResultContract[]) => void ) => void;

        public getOne: (id: number, callback: (result: dc.ArtistContract) => void) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(baseUrl: string) {

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Artist") + relative;
            };

            this.findDuplicate = (params, callback: (result: dc.DuplicateEntryResultContract[]) => void ) => {

                $.post(this.mapUrl("/FindDuplicate"), params, callback);

            }

            this.getOne = (id: number, callback: (result: dc.ArtistContract) => void) => {
                
                $.post(this.mapUrl("/DataById"), { id: id }, callback);
                    
            }

        }

    }

}