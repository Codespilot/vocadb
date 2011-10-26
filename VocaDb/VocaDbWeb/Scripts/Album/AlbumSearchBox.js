
function initAddForm(addName, addButton, albumList, allowAddNew) {

	addName.keyup(function () {

		var findTerm = $(this).val();

		if (findTerm.length == 0) {

			$(albumList).empty();
			return;

		}

		$.post("../../Album/FindJson", { term: findTerm }, function (results) {

			$(albumList).empty();

			$(results).each(function () {

				addOption(albumList, this.Id, this.Name);

			});

			if (allowAddNew)
				addOption(albumList, "", "Create new album named '" + findTerm + "'");

		});

	});

	addButton.click(function () {

		var findTerm = addName.val();

		if (findTerm.length == 0)
			return;

		var albumId = $(albumList).val();

		if (albumId == "") {
			$.post("../../Song/AddNewArtist", { songId: songId, newArtistName: findTerm }, artistAdded);
		} else {
			$.post("../../Song/AddExistingArtist", { songId: songId, albumId: albumId }, artistAdded);
		}

	});

}

