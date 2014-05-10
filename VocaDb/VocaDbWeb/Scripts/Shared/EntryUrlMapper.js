var vdb;
(function (vdb) {
    (function (utils) {
        var EntryUrlMapper = (function () {
            function EntryUrlMapper() {
            }
            EntryUrlMapper.details = function (typeName, id) {
                switch (typeName.toLowerCase()) {
                    case "album":
                        return vdb.functions.mapAbsoluteUrl("/Al/" + id);
                    case "artist":
                        return vdb.functions.mapAbsoluteUrl("/Ar/" + id);
                    case "song":
                        return vdb.functions.mapAbsoluteUrl("/S/" + id);
                }

                return vdb.functions.mapAbsoluteUrl("/" + typeName + "/Details/" + id);
            };

            EntryUrlMapper.details_entry = function (entry) {
                return EntryUrlMapper.details(entry.entryType, entry.id);
            };

            EntryUrlMapper.details_tag_byName = function (name) {
                return vdb.functions.mapAbsoluteUrl("/Tag/Details/" + name);
            };
            return EntryUrlMapper;
        })();
        utils.EntryUrlMapper = EntryUrlMapper;
    })(vdb.utils || (vdb.utils = {}));
    var utils = vdb.utils;
})(vdb || (vdb = {}));
