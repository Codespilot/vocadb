var vdb;
(function (vdb) {
    (function (tests) {
        (function (utils) {
            var uti = vdb.utils;

            QUnit.module("WebLinkMatcher");

            test("matchWebLink match", function () {
                var result = uti.WebLinkMatcher.matchWebLink("http://www.youtube.com/user/tripshots");

                ok(result, "result");
                equal(result.desc, "Youtube Channel", "desc");
                equal(result.cat, vdb.models.WebLinkCategory.Official, "cat");
            });

            test("matchWebLink no match", function () {
                var result = uti.WebLinkMatcher.matchWebLink("http://www.google.com");

                equal(result, null, "result");
            });
        })(tests.utils || (tests.utils = {}));
        var utils = tests.utils;
    })(vdb.tests || (vdb.tests = {}));
    var tests = vdb.tests;
})(vdb || (vdb = {}));
