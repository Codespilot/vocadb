/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

	import dc = vdb.dataContracts;

    // Repository for managing albums and related objects.
    // Corresponds to the AlbumController class.
    export class AlbumRepository {

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(private baseUrl: string) {

            this.mapUrl = (relative) => {
                return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
            };

		}

		getList = (paging: dc.PagingProperties, lang: string, query: string, sort: string,
			discTypes: string, tag: string,
			artistId: number, artistParticipationStatus: string,
			callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxEntries: paging.maxEntries,
				query: query, fields: "MainPicture", lang: lang, nameMatchMode: 'Auto', sort: sort,
				discTypes: discTypes,
				tag: tag,
				artistId: artistId,
				artistParticipationStatus: artistParticipationStatus
			};

			$.getJSON(url, data, callback);

		}

    }

}