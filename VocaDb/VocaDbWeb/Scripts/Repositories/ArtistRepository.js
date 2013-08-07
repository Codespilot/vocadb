var vdb;
(function (vdb) {
    /// <reference path="../typings/jquery/jquery.d.ts" />
    /// <reference path="../Shared/GlobalFunctions.ts" />
    /// <reference path="../DataContracts/ArtistContract.ts" />
    /// <reference path="../DataContracts/DuplicateEntryResultContract.ts" />
    (function (repositories) {
        

        // Repository for managing artists and related objects.
        // Corresponds to the ArtistController class.
        var ArtistRepository = (function () {
            function ArtistRepository(baseUrl) {
                var _this = this;
                this.mapUrl = function (relative) {
                    return vdb.functions.mergeUrls(baseUrl, "/Artist") + relative;
                };

                this.findDuplicate = function (params, callback) {
                    $.post(_this.mapUrl("/FindDuplicate"), params, callback);
                };

                this.getOne = function (id, callback) {
                    $.post(_this.mapUrl("/DataById"), { id: id }, callback);
                };
            }
            return ArtistRepository;
        })();
        repositories.ArtistRepository = ArtistRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
