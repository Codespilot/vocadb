var vdb;
(function (vdb) {
    (function (models) {
        (function (EntryType) {
            EntryType[EntryType["Undefined"] = 0] = "Undefined";

            EntryType[EntryType["Album"] = 1] = "Album";

            EntryType[EntryType["Artist"] = 2] = "Artist";

            EntryType[EntryType["PV"] = 4] = "PV";

            EntryType[EntryType["ReleaseEvent"] = 8] = "ReleaseEvent";

            EntryType[EntryType["ReleaseEventSeries"] = 16] = "ReleaseEventSeries";

            EntryType[EntryType["Song"] = 32] = "Song";

            EntryType[EntryType["SongList"] = 64] = "SongList";

            EntryType[EntryType["Tag"] = 128] = "Tag";

            EntryType[EntryType["User"] = 256] = "User";
        })(models.EntryType || (models.EntryType = {}));
        var EntryType = models.EntryType;
    })(vdb.models || (vdb.models = {}));
    var models = vdb.models;
})(vdb || (vdb = {}));
