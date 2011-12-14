function initEntrySearch(nameBoxElem, idElem, findListElem, acceptElem, allowCreateNew, entityName, searchUrl, createOptionHtml, createTitle, acceptSelection) {

	$(findListElem).jqxListBox({ width: '400', height: '350' });

	$(findListElem).bind('select', function (event) {

		var item = $(findListElem).jqxListBox('getItem', args.index);

		if (item != null) {
			$(idElem).val(item.value);
		}

	});

	$(nameBoxElem).keyup(function () {

		var findTerm = $(this).val();

		if (isNullOrWhiteSpace(findTerm)) {

			$(findListElem).jqxListBox({ source: new Array() });
			return;

		}

		$.post(searchUrl, { term: findTerm }, function (results) {

			if ($(nameBoxElem).val() != results.Term)
				return;

			$(findList).empty();
			var rows = new Array();

			$(results.Items).each(function () {

				var html = createOptionHtml(this);

				if (html != null)
					rows.push({ value: this.Id, html: html, title: createTitle(this) });

			});

			if (allowCreateNew)
				rows.push({ value: "", html: "<div tabIndex=0 style='padding: 1px;'><div>Create new " + entityName + " named '" + findTerm + "'</div></div>" });

			$(findListElem).jqxListBox({ source: rows });

		});

	});

	$(acceptElem).click(function () {

		var findTerm = $(nameBoxElem).val();

		if (isNullOrWhiteSpace(findTerm))
			return;

		var selectedId = $(findListElem).val();

		$(nameBoxElem).val("");
		$(findListElem).jqxListBox({ source: new Array() });

		acceptSelection(selectedId, findTerm);

	});

}