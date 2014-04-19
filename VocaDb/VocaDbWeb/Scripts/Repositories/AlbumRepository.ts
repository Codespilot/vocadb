/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

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

		getList = (start: number, query: string, sort: string, callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums");
			var data = { start: start, query: query, fields: "MainPicture", lang: 'English', nameMatchMode: 'Words', sort: sort };

			$.getJSON(url, data, callback);

		}

    }

}