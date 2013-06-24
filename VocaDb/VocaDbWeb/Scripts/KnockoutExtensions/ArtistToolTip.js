ko.bindingHandlers.artistToolTip = {
    init: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());

        $(element).qtip({
            content: {
                text: 'Loading...',
                ajax: {
                    url: vdb.functions.mapAbsoluteUrl('/Artist/PopupContent'),
                    type: 'GET',
                    data: { id: value }
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
};
