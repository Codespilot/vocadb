function onChangeAlbumMediaType() {

	var id = getId(this);
	$.post("../User/UpdateAlbumForUserMediaType", { albumForUserId: id, mediaType: $(this).val() });

}

function initPage(userId) {

	$("#tabs").tabs({
		load: function (event, ui) {

			$("#tabs").tabs("url", ui.index, "");

			$(".collectionRating").jqxRating();

			$(".ratingVal").each(function () {

				var val = $(this).val();

				$(this).parent().find(".collectionRating").jqxRating({ value: val });

			});

			$(".collectionRating").bind('change', function (event) {

				var id = getId($(this).parent().parent());
				var val = event.value;

				$.post("../User/UpdateAlbumForUserRating", { albumForUserId: id, rating: val });

			});

			function acceptAlbumSelection(albumId, term) {

				if (!isNullOrWhiteSpace(albumId)) {
					$.post("../../User/AddExistingAlbum", { albumId: albumId }, albumAdded);
				}

			}

			var albumAddList = $("#albumAddList");
			var albumAddName = $("input#albumAddName");
			var albumAddBtn = $("#albumAddAcceptBtn");

			initEntrySearch(albumAddName, albumAddList, "Album", "../../Album/FindJson", {
				allowCreateNew: false,
				acceptBtnElem: albumAddBtn,
				acceptSelection: acceptAlbumSelection,
				createOptionFirstRow: function (item) { return item.Name },
				createOptionSecondRow: function (item) { return item.ArtistString },
				createTitle: function (item) { return item.AdditionalNames }
			});

			$("select.albumMediaType").change(onChangeAlbumMediaType);

		}
	});

	function albumAdded(row) {

		$("#albumTableBody").append(row);
		var newRow = $("#albumTableBody tr:last");
		$(newRow).find("select.albumMediaType").change(onChangeAlbumMediaType);
		/*var addRow = $("#albumRow_new");
		addRow.before(row);
		var newRow = $(addRow).prev();
		$(newRow).find("select.albumMediaType").change(onChangeAlbumMediaType);
		$("input#albumAddName").val("");
		$("#albumAddList").empty();*/

	}

	$("a.albumRemove").live("click", function () {

		var id = getId(this);
		$.post("../User/DeleteAlbumForUser", { albumForUserId: id });
		$(this).parent().parent().remove();

		return false;

	});

}