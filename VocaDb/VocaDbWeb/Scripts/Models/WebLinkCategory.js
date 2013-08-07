var vdb;
(function (vdb) {
    (function (models) {
        (function (WebLinkCategory) {
            WebLinkCategory[WebLinkCategory["Official"] = 0] = "Official";
            WebLinkCategory[WebLinkCategory["Commercial"] = 1] = "Commercial";
            WebLinkCategory[WebLinkCategory["Reference"] = 2] = "Reference";
            WebLinkCategory[WebLinkCategory["Other"] = 3] = "Other";
        })(models.WebLinkCategory || (models.WebLinkCategory = {}));
        var WebLinkCategory = models.WebLinkCategory;

        function parseWebLinkCategory(rating) {
            switch (rating) {
                case "Official":
                    return WebLinkCategory.Official;
                case "Commercial":
                    return WebLinkCategory.Commercial;
                case "Reference":
                    return WebLinkCategory.Reference;
                case "Other":
                    return WebLinkCategory.Other;
            }
        }
        models.parseWebLinkCategory = parseWebLinkCategory;
    })(vdb.models || (vdb.models = {}));
    var models = vdb.models;
})(vdb || (vdb = {}));
