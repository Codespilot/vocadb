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

		public getList = (paging: dc.PagingProperties, query: string, sort: string, callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxEntries: paging.maxEntries,
				query: query, fields: "MainPicture", lang: 'English', nameMatchMode: 'Words', sort: sort
			};

			$.getJSON(url, data, callback);

		}

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(private baseUrl: string) {

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