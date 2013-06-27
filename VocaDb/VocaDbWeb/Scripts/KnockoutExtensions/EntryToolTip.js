var vdb;
(function (vdb) {
    (function (knockoutExtensions) {
        function initToolTip(element, relativeUrl, id) {
            $(element).qtip({
                content: {
                    text: 'Loading...',
                    ajax: {
                        url: vdb.functions.mapAbsoluteUrl(relativeUrl),
                        type: 'GET',
                        data: { id: id }
                    }
                },
                position: {
                    viewport: $(window)
                },
                style: {
                    classes: "tooltip-wide"
                }
            });
        }
        knockoutExtensions.initToolTip = initToolTip;
    })(vdb.knockoutExtensions || (vdb.knockoutExtensions = {}));
    var knockoutExtensions = vdb.knockoutExtensions;
})(vdb || (vdb = {}));

ko.bindingHandlers.entryToolTip = {
    init: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());

        switch (value.entryType) {
            case "Artist":
                vdb.knockoutExtensions.initToolTip(element, '/Artist/PopupContent', value.id);
                break;
        }
    }
};

ko.bindingHandlers.artistToolTip = {
    init: function (element, valueAccessor) {
        var id = ko.utils.unwrapObservable(valueAccessor());
        vdb.knockoutExtensions.initToolTip(element, '/Artist/PopupContent', id);
    }
};
