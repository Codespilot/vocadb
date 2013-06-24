var vdb;
(function (vdb) {
    (function (repositories) {
        var dc = vdb.dataContracts;

        var SongRepository = (function () {
            function SongRepository(baseUrl) {
                var _this = this;
                this.mapUrl = function (relative) {
                    return baseUrl + "/Song" + relative;
                };

                this.findDuplicate = function (params, callback) {
                    $.post(_this.mapUrl("/FindDuplicate"), params, callback);
                };
            }
            return SongRepository;
        })();
        repositories.SongRepository = SongRepository;
    })(vdb.repositories || (vdb.repositories = {}));
    var repositories = vdb.repositories;
})(vdb || (vdb = {}));
