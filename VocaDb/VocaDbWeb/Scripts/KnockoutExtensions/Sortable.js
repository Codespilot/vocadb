
ko.bindingHandlers.sortable = {
    init: function (element, valueAccessor) {
        var list = valueAccessor();
        $(element).sortable({
            update: function (event, ui) {
                var data = ko.dataFor(ui.item[0]);
                var index = ui.item.index();
                if (index > 0) {
                    list.remove(data);
                    list.splice(index, 0, data);
                }
            }
        });
    }
};
