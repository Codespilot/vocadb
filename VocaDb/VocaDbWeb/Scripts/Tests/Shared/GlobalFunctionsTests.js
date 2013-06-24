var vdb;
(function (vdb) {
    (function (tests) {
        (function (functions) {
            var fu = vdb.functions;

            QUnit.module("GlobalFunctions");

            function mergeUrls(base, relative) {
                return fu.mergeUrls(base, relative);
            }

            test("mergeUrls bothWithSlash", function () {
                var result = mergeUrls("/", "/Song");

                equal(result, "/Song", "result");
            });

            test("mergeUrls baseWithSlash", function () {
                var result = mergeUrls("/", "Song");

                equal(result, "/Song", "result");
            });

            test("mergeUrls relativeWithSlash", function () {
                var result = mergeUrls("", "/Song");

                equal(result, "/Song", "result");
            });

            test("mergeUrls neitherWithSlash", function () {
                var result = mergeUrls("", "Song");

                equal(result, "/Song", "result");
            });
        })(tests.functions || (tests.functions = {}));
        var functions = tests.functions;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
