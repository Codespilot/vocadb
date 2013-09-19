/// <reference path="GlobalFunctions.ts" />

module vdb {

    export class UrlMapper {

        constructor(public baseUrl: string) { }

        public mapRelative(relative: string) {
            return vdb.functions.mergeUrls(this.baseUrl, relative);
        }

    }

}