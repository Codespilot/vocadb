var vdb;
(function (vdb) {
    (function (values) {
        // URL of the site path, for example "/"
        values.baseAddress;

        // URL including the scheme and site path, for example "http://vocadb.net/"
        values.hostAddress;

        values.webLinkMatchers;
    })(vdb.values || (vdb.values = {}));
    var values = vdb.values;
})(vdb || (vdb = {}));
