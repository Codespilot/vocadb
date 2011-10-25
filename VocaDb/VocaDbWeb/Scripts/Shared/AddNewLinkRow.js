function initAddNewLinkRowCtl(prefix, entityName, searchUrl) {

	var findListId = "#" + prefix + "AddList";
	var nameBoxId = "input#" + prefix + "AddName";

	$(nameBoxId).keyup(function () {

		var findList = $(findListId);
		var findTerm = $(this).val();

		if (findTerm.length == 0) {

			$(findList).empty();
			return;

		}

		$.post(searchUrl, { term: findTerm }, function (results) {

			$(findList).empty();

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