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
            if (relative.length && relative.charAt(0) == '/')
                relative = relative.substr(1);

            return vdb.values.baseAddress + relative;
        }
        functions.mapAbsoluteUrl = mapAbsoluteUrl;
        ;

        function mapFullUrl(relative) {
            if (relative.length && relative.charAt(0) == '/')
                relative = relative.substr(1);

            return vdb.values.hostAddress + relative;
        }
        functions.mapFullUrl = mapFullUrl;
        ;
    })(vdb.functions || (vdb.functions = {}));
    var functions = vdb.functions;
})(vdb || (vdb = {}));
