function initEntrySearch(nameBoxElem, findListElem, entityName, searchUrl, params) {

	function createHtml(item) {

		var html = null;

		if (createOptionHtml) {
			html = createOptionHtml(item);
		} else if (createOptionFirstRow && createOptionSecondRow) {
			var firstRow = createOptionFirstRow(item);
			var secondRow = createOptionSecondRow(item);
			if (firstRow)
				html = "<a><div>" + firstRow + "</div><div>" + secondRow + "</div></a>";
		} else if (createOptionFirstRow) {
			var firstRow = createOptionFirstRow(item);
			if (firstRow)
				html = "<a><div>" + firstRow + "</div></a>";
		}

		return html;

	}

	function getItems(par, response) {

		var queryParams = { term: par.term };

		if (extraQueryParams != null)
			jQuery.extend(queryParams, extraQueryParams);

		$.post(searchUrl, queryParams, function (result) {

			var mapped = $.map(result.Items, function (item) {
				return { label: item.Name, value: item.Id, data: item };
			});

			response(mapped);

		});

	}

	function selectItem(event, ui) {

		acceptSelection(ui.item.Id, $(nameBoxElem).val());
		$(nameBoxElem).val("");

		return false;

	}

	var w = 400;
	var h = 350;
	var idElem = null;
	var allowCreateNew = false;
	var acceptSelection = null;
	var extraQueryParams = null;
	var createOptionFirstRow = null;
	var createOptionSecondRow = null;
	var createOptionHtml = null;
	var createTitle = null;

	if (params != null) {

		if (params.width != null)
			w = params.width;

		if (params.height != null)
			h = params.height;

		if (params.allowCreateNew != null)
			allowCreateNew = params.allowCreateNew;

		if (params.acceptSelection != null)
			acceptSelection = params.acceptSelection;

		if (params.idElem != null)
			idElem = params.idElem;

		if (params.extraQueryParams != null)
			extraQueryParams = params.extraQueryParams;

		createOptionFirstRow = params.createOptionFirstRow;
		createOptionSecondRow = params.createOptionSecondRow;
		createOptionHtml = params.createOptionHtml;
		createTitle = params.createTitle;

	}

	$(nameBoxElem).autocomplete({
		source: getItems,
		select: selectItem
	})
	.data("autocomplete")._renderItem = function (ul, item) {
		return $("<li>")
			.data("item.autocomplete", item.data)
			.append(createHtml(item.data))
			.appendTo(ul);
	};

}