var vdb;
(function (vdb) {
    (function (functions) {
        function isNullOrWhiteSpace(str) {
            if (str == null || str.length == 0)
                return true;

            return !(/\S/.test(str));
        }
        functions.isNullOrWhiteSpace = isNullOrWhiteSpace;

        function mapAbsoluteUrl(relative) {
            return mergeUrls(vdb.values.baseAddress, relative);
        }
        functions.mapAbsoluteUrl = mapAbsoluteUrl;
        ;

        function mapFullUrl(relative) {
            return mergeUrls(vdb.values.hostAddress, relative);
        }
        functions.mapFullUrl = mapFullUrl;
        ;

        function mergeUrls(base, relative) {
            if (base.charAt(base.length - 1) == "/" && relative.charAt(0) == "/")
                return base + relative.substr(1);

            if (base.charAt(base.length - 1) == "/" && relative.charAt(0) != "/")
                return base + relative;

            if (base.charAt(base.length - 1) != "/" && relative.charAt(0) == "/")
                return base + relative;

            return base + "/" + relative;
        }
        functions.mergeUrls = mergeUrls;
    })(vdb.functions || (vdb.functions = {}));
    var functions = vdb.functions;
})(vdb || (vdb = {}));
