function initLinkRowCtl(nameBoxElem, idElem, findListElem, acceptElem, allowCreateNew, entityName, searchUrl, createOptionHtml, acceptSelection) {

	$(findListElem).jqxListBox({ width: '400', height: '350' });

	$(findListElem).bind('select', function (event) {

		var item = $(findListElem).jqxListBox('getItem', args.index);

		if (item != null) {
			$(idElem).val(item.value);
		}

	});

	$(nameBoxElem).keyup(function () {

		var findList = $(findListId);
		var findTerm = $(this).val();

		if (isNullOrWhiteSpace(findTerm.length)) {

			$(findList).empty();
			return;

		}

		$.post(searchUrl, { term: findTerm }, function (results) {

			$(findList).empty();
			var rows = new Array();

			$(results).each(function () {

				addOption(findList, this.Id, this.Name);

			});

			addOption(findList, "", "Create new " + entityName + " named '" + findTerm + "'");

		});

	});

	$("#" + prefix + "AddBtn").click(function () {

		var findTerm = $(nameBoxId).val();
		var findList = $(findListId);

		if (findTerm.length == 0)
			return;

		var selectedId = $(findList).val();

		if (selectedId == "") {
			$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: findTerm }, rowAdded);
		} else {
			$.post("../../Album/AddExistingArtist", { albumId: albumId, artistId: artistId }, rowAdded);
		}

	});

	function rowAdded(row) {

		var addRow = $("#" + prefix + "Row_new");
		addRow.before(row);
		$(nameBoxId).val("");
		$(findListId).empty();

	}

}