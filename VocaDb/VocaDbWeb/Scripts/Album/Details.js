
function saveTagSelections() {

	

}

function tabLoaded(albumId, event, ui) {

	$("#tabs").tabs("url", 1, "");

	$("#createComment").click(function () {

		var message = $("#newCommentMessage").val();

		if (message == "") {
			alert("Message cannot be empty");
			return false;
		}

		$("#newCommentMessage").val("");

		$.post("../../Album/CreateComment", { albumId: albumId, message: message }, function (result) {

			$("#newCommentPanel").after(result);

		});

		return false;

	});

	$("a.deleteComment").live("click", function () {

		if (!confirm("Are you sure you want to delete this comment?"))
			return false;

		var btn = this;
		var id = getId(this);

		$.post("../../Album/DeleteComment", { commentId: id }, function () {

			$(btn).parent().parent().remove();

		});

		return false;

	});

}

function initPage(albumId) {

	$("#addAlbumLink").button({ icons: { primary: 'ui-icon-star'} });
	$("#removeAlbumLink").button({ icons: { primary: 'ui-icon-close'} });
	$("#editAlbumLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#editTags").button({ icons: { primary: 'ui-icon-tag'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#downloadTags").button({ icons: { primary: 'ui-icon-arrowthickstop-1-s'} });

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: { "Save": saveTagSelections } });

	$("#tabs").tabs({
		load: function(event, ui) {
			tabLoaded(albumId, event, ui);
		}
	});

	$("#addAlbumLink").click(function () {

		$.post("../../User/AddExistingAlbum", { albumId: albumId }, function (result) {

			$("#removeAlbumLink").show();
			$("#addAlbumLink").hide();

		});

		return false;

	});

	$("#removeAlbumLink").click(function () {

		$.post("../../User/RemoveAlbumFromUser", { albumId: albumId }, function (result) {

			$("#addAlbumLink").show();
			$("#removeAlbumLink").hide();

		});

		return false;

	});

	$("#editTags").click(function () {

		$.get("../../Album/TagSelections", { albumId: albumId }, function (content) {

			$("#editTagsContent").html(content);

			$("input.tagSelection").button();

			$("#editTagsPopup").dialog("open");

		});

		return false;		

	});

}