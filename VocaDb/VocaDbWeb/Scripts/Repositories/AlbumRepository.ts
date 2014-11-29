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

		public getForEdit = (id: number, callback: (result: dc.albums.AlbumForEditContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

		getList = (paging: dc.PagingProperties, lang: string, query: string, sort: string,
			discTypes: string, tag: string,
			artistId: number, artistParticipationStatus: string,
			childVoicebanks: boolean,
			fields: string,
			status: string,
			callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort,
				discTypes: discTypes,
				tag: tag,
				artistId: artistId,
				artistParticipationStatus: artistParticipationStatus,
				childVoicebanks: childVoicebanks,
				status: status
			};

			$.getJSON(url, data, callback);

		}

    }

}