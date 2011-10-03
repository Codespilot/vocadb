function initPage(userId) {

	$("#tabs").tabs();	

	$("input#albumAddName").keyup(function () {

		var findTerm = $(this).val();
		var albumList = $("#albumAddList");

		if (findTerm.length < 3) {

			$(albumList).empty();

			if (findTerm.length > 0) {
				addOption(albumList, "", "Create new album named '" + findTerm + "'");
			}

			return;
		}

		$.post("../../Album/FindJson", { term: findTerm }, function (results) {

			$(albumList).empty();

			$(results).each(function () {

				addOption(albumList, this.Id, this.Name);

			});

			addOption(albumList, "", "Create new album named '" + findTerm + "'");

		});

	});

	$("#albumAddBtn").click(function () {

		var findTerm = $("input#albumAddName").val();
		var albumList = $("#albumAddList");

		if (findTerm.length == 0)
			return;

		var albumId = $(albumList).val();

		if (albumId == "") {
			$.post("../../User/AddNewAlbum", { newAlbumName: findTerm }, albumAdded);
		} else {
			$.post("../../User/AddExistingAlbum", { albumId: albumId }, albumAdded);
		}

	});

	function albumAdded(row) {

		var addRow = $("#albumRow_new");
		addRow.before(row);
		$("input#albumAddName").val("");
		$("#albumAddList").empty();

	}

	$("input.albumRemove").live("click", function () {

		var id = getId(this);
		$.post("../../User/DeleteAlbumForUser", { albumForUserId: id }, function () {

			$("tr#albumRow_" + id).remove();

		});

	});

}