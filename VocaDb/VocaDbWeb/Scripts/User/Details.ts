import rep = vdb.repositories;

function initPage(userId, loadingStr, confirmDisableStr, hostAddress: string) {

	$("#tabs").tabs({
		load: function (event, ui) {

			vdb.functions.disableTabReload(ui.tab);
			$("#tabs").tabs("option", "spinner", loadingStr);

		}
	});

	$("#mySettingsLink").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#messagesLink").button({ icons: { primary: 'ui-icon-mail-closed' } });
	$("#composeMessageLink").button({ icons: { primary: 'ui-icon-mail-closed' } });
	$("#editUserLink").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#disableUserLink").button({ icons: { primary: 'ui-icon-close' } });
	$("#avatar").tooltip({ placement: "bottom" });

	$("#disableUserLink").click(function () {

		return confirm(confirmDisableStr);

	});

	$("#removeRating").click(function () {

		$('#collectionRating').jqxRating('setValue', 0);
		return false;

	});

	$("#editCollectionDialog").dialog({
		autoOpen: false, width: 320, modal: true, buttons: [{
		text: "Save", click: function () {

		$("#editCollectionDialog").dialog("close");

		var albumId = $("#collectionAlbumEditId").val();
		var status = $("#collectionStatusSelect").val();
		var mediaType = $("#collectionMediaSelect").val();
		var rating = $("#collectionRating").jqxRating('getValue');

		$.post("../../User/UpdateAlbumForUser", { albumId: albumId, collectionStatus: status, mediaType: mediaType, rating: rating }, null);

		var idField = $("#albumCollectionTableBody input.albumId[value='" + albumId + "']");
		var dataRow = $(idField).parent().parent();
		var dataCol = $(dataRow).find("td")[0];
		$(dataCol).find(".albumPurchaseStatus").val(status);
		$(dataCol).find(".albumMediaType").val(mediaType);
		$(dataCol).find(".albumRating").val(rating);
		var collectionName = $("#collectionStatusSelect option:selected").text();
		var mediaName = $("#collectionMediaSelect option:selected").text();
		$(dataRow).find(".albumPurchaseStatusField").text(collectionName);
		$(dataRow).find(".albumMediaTypeField").text(mediaName);

		$.get("../../Shared/Stars", { current: rating, max: 5 }, function (res) {
			$(dataRow).find(".albumRatingField").html(res);
		});

	}
	}]
	});

	$("#tabs").on("click", ".editAlbumLink", function () {

		var dataCol = $(this).parent().parent().find("td")[0];
		var albumId = $(dataCol).find(".albumId").val();
		var albumPurchaseStatus = $(dataCol).find(".albumPurchaseStatus").val();
		var albumMediaType = $(dataCol).find(".albumMediaType").val();
		var albumRating = $(dataCol).find(".albumRating").val();

		$("#collectionAlbumEditId").val(albumId);
		$("#collectionStatusSelect").val(albumPurchaseStatus);
		$("#collectionMediaSelect").val(albumMediaType);
		$('#collectionRating').jqxRating({ value: albumRating });
		$('#collectionRating').jqxRating('setValue', albumRating);

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	// Comments
	$("#createComment").click(function () {

		var message = $("#newCommentMessage").val();

		if (message == "") {
			alert("Message cannot be empty");
			return false;
		}

		$("#newCommentMessage").val("");

		$.post(hostAddress + "/User/CreateComment", { entryId: userId, message: message }, function (result) {

			$("#comments").prepend(result);

		});

		return false;

	});

	$(document).on("click", "a.deleteComment", function () {

		if (!confirm("Are you sure you want to delete this comment?"))
			return false;

		var btn = this;
		var id = vdb.functions.getId(this);

		$.post(hostAddress + "/User/DeleteComment", { commentId: id }, function () {

			$(btn).parent().parent().parent().parent().remove();

		});

		return false;

	});

	$("#sfsCheckDialog").dialog({ autoOpen: false, model: true });
	$("#favoriteAlbums img").vdbAlbumToolTip();

}