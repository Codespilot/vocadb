var vdb;
(function (vdb) {
    (function (functions) {
        function isNullOrWhiteSpace(str) {
            if (str == null || str.length == 0)
                return true;

            return !(/\S/.test(str));
        }
        functions.isNullOrWhiteSpace = isNullOrWhiteSpace;
    })(vdb.functions || (vdb.functions = {}));
    var functions = vdb.functions;
})(vdb || (vdb = {}));
