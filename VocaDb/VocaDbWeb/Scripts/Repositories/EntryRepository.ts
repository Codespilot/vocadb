﻿
module vdb.repositories {

	import dc = vdb.dataContracts;

	// Repository for finding base class of common entry types.
	// Corresponds to the EntryApiController.
	export class EntryRepository {

		// Maps a relative URL to an absolute one.
		private mapUrl = (relative: string) => {
			return vdb.functions.mergeUrls(vdb.functions.mergeUrls(this.baseUrl, "/api/entries"), relative);
		};

		constructor(private baseUrl: string) {

		}

		getList = (paging: dc.PagingProperties, lang: string, query: string, tag: string, status: string, callback) => {

			var url = this.mapUrl("");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxEntries: paging.maxEntries,
				query: query, fields: "MainPicture", lang: lang, nameMatchMode: 'Auto',
				tag: tag,
				status: status
			};

			$.getJSON(url, data, callback);

		}

    }

}