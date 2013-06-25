var vdb;
(function (vdb) {
    (function (utils) {
        var EntryUrlMapper = (function () {
            function EntryUrlMapper() {
            }
            EntryUrlMapper.details = function (typeName, id) {
                return vdb.functions.mapAbsoluteUrl("/" + typeName + "/Details/" + id);
            };

            EntryUrlMapper.details_entry = function (entry) {
                return EntryUrlMapper.details(entry.entryType, entry.id);
            };
            return EntryUrlMapper;
        })();
        utils.EntryUrlMapper = EntryUrlMapper;
    })(vdb.utils || (vdb.utils = {}));
    var utils = vdb.utils;
})(vdb || (vdb = {}));
