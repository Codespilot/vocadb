/*var EntrySearchParams = function () {

	this.width = 400;
	this.height = 350;

};

function initEntrySearchP(params) {

	var nameBoxElem = params.nameBoxElem;
	var idElem = params.idElem;
	var findListElem = params.findListElem;
	var acceptBtnElem = params.acceptBtnElem;
	var allowCreateNew = params.allowCreateNew;
	var entityName = params.entityName;
	var searchUrl = params.searchUrl;
	var createOptionHtml = params.createOptionHtml;
	var createTitle = params.createTitle;
	var acceptSelection = params.acceptSelection;

	initEntrySearch(nameBoxElem, idElem, findListElem, acceptBtnElem, allowCreateNew, entityName, searchUrl, createOptionHtml, createTitle, acceptSelection);

}*/

function clearFindList(findListElem) {

	$(findListElem).jqxListBox('selectIndex', -1);
	var rows = new Array();
	rows.push({ value: "", html: "Nothing" });
	$(findListElem).jqxListBox({ disabled: true });
	$(findListElem).jqxListBox({ source: rows });
	//$(findListElem).jqxListBox({ source: new Array() });
	//$(findListElem).jqxListBox({ source: new Array() });	

}

function initEntrySearch(nameBoxElem, findListElem, entityName, searchUrl, createOptionHtml, createTitle, params) {

	var w = 400;
	var h = 350;
	var idElem = null;
	var acceptBtnElem = null;
	var allowCreateNew = false;
	var acceptSelection = null;

	if (params != null) {

		if (params.width != null)
			w = params.width;

		if (params.height != null)
			h = params.height;

		if (params.acceptBtnElem != null)
			acceptBtnElem = params.acceptBtnElem;

		if (params.allowCreateNew != null)
			allowCreateNew = params.allowCreateNew;

		if (params.acceptSelection != null)
			acceptSelection = params.acceptSelection;

		if (params.idElem != null)
			idElem = params.idElem;

	}

	$(findListElem).jqxListBox({ width: w, height: h });

	if (idElem != null) {
		$(findListElem).bind('select', function (event) {

			var item = $(findListElem).jqxListBox('getItem', args.index);

			if (item != null) {
				$(idElem).val(item.value);
			}

		});
	}

	$(nameBoxElem).keyup(function () {

		var findTerm = $(this).val();

		if (isNullOrWhiteSpace(findTerm)) {

			clearFindList(findListElem);
			return;

		}

		$.post(searchUrl, { term: findTerm }, function (results) {

			if (results.Term != null && $(nameBoxElem).val() != results.Term)
				return;

			var rows = new Array();

			$(results.Items).each(function () {

				var html = createOptionHtml(this);

				if (html != null)
					rows.push({ value: this.Id, html: html, title: createTitle(this) });

			});

			if (allowCreateNew)
				rows.push({ value: "", html: "<div tabIndex=0 style='padding: 1px;'><div>Create new " + entityName + " named '" + findTerm + "'</div></div>" });

			if (rows.length == 0) {
				rows.push({ value: "", html: "Nothing" });
				$(findListElem).jqxListBox({ disabled: true });
			} else {
				$(findListElem).jqxListBox({ disabled: false });
			}

			$(findListElem).jqxListBox('selectIndex', -1);
			$(findListElem).jqxListBox('ensureVisible', 0);
			$(findListElem).jqxListBox({ source: rows });

		});

	});

	$(nameBoxElem).bind("paste", function (e) {
		var elem = $(this);
		setTimeout(function () {
			$(elem).trigger("keyup");
		}, 0);
	});

	if (acceptBtnElem != null) {
		$(acceptBtnElem).click(function () {

			var findTerm = $(nameBoxElem).val();

			if (isNullOrWhiteSpace(findTerm))
				return;

			var selectedId = "";

			var selectedIndex = $(findListElem).jqxListBox('getSelectedIndex');
			var item = null;

			item = $(findListElem).jqxListBox('getItem', selectedIndex);

			if (item != null && item.value != null) {
				selectedId = item.value;
			}

			$(nameBoxElem).val("");
			clearFindList(findListElem);

			acceptSelection(selectedId, findTerm);

		});
	}

}