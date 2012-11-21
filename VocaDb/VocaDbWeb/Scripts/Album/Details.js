
function initDialog() {

	$("input.tagSelection").button();

	$("input#newTagName").autocomplete({
		source: "../../Tag/Find"
	});

	$("input#addNewTag").click(function () {

		var name = $("#newTagName").val();

		if (name == "")
			return false;

		$("#newTagName").val("");

		$.post("../../Tag/Create", { name: name }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
			} else {
				$("#tagSelections").append(response.Result);
				$("input.tagSelection").button();
			}

		});

		return false;

	});

}

function saveTagSelections() {

	var tagNames = new Array();

	$("input.tagSelection:checked").each(function () {
		var name = $(this).parent().find("input.tagName").val();
		tagNames.push(name);
	});

	var tagNamesStr = tagNames.join(",");
	var albumId = $("#editTagsAlbumId").val();

	$.post("../../Album/TagSelections", { albumId: albumId, tagNames: tagNamesStr }, function (content) {

		$("#tagList").html(content);

	});

	$("#editTagsPopup").dialog("close");

}

function tabLoaded(albumId, event, ui, confirmDeleteStr) {

	$("#tabs").tabs("url", 1, "");

	$("#createComment").click(function () {

		var message = $("#newCommentMessage").val();

		if (message == "") {
			alert("Message cannot be empty");
			return false;
		}

		$("#newCommentMessage").val("");

		$.post("../../Album/CreateComment", { entryId: albumId, message: message }, function (result) {

			$("#newCommentPanel").after(result);

		});

		return false;

	});

	$("a.deleteComment").live("click", function () {

		if (!confirm(confirmDeleteStr))
			return false;

		var btn = this;
		var id = getId(this);

		$.post("../../Album/DeleteComment", { commentId: id }, function () {

			$(btn).parent().parent().remove();

		});

		return false;

	});

}

function initPage(albumId, collectionRating, saveStr, confirmDeleteStr, hostAddress) {

	$("#addAlbumLink").button({ disabled: $("#addAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-star'} });
	$("#updateAlbumLink").button({ disabled: $("#updateAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#editAlbumLink").button({ disabled: $("#editAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#downloadTags").button({ icons: { primary: 'ui-icon-arrowthickstop-1-s'} });
	$("#editTags").button({ disabled: $("#editTags").hasClass("disabled"), icons: { primary: 'ui-icon-tag'} });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("select", 1);
		return false;
	});

	$('#picCarousel').carousel({ interval: false });

	$("#collectionRating").jqxRating();

	if (collectionRating != 0) {
		$('#collectionRating').jqxRating({ value: collectionRating });
	}

	$("#removeRating").click(function () {

		$('#collectionRating').jqxRating({ value: 0 });

		return false;

	});

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: [{ text: saveStr, click: saveTagSelections }] });

	$("#tabs").tabs({
		load: function(event, ui) {
			tabLoaded(albumId, event, ui, confirmDeleteStr);
		}
	});

	$("#editCollectionDialog").dialog({ autoOpen: false, width: 320, modal: false, buttons: [{ text: saveStr, click: function () {

		$("#editCollectionDialog").dialog("close");

		var status = $("#collectionStatusSelect").val();
		var mediaType = $("#collectionMediaSelect").val();
		var rating = $("#collectionRating").jqxRating('getValue');

		$.post("../../User/UpdateAlbumForUser", { albumId: albumId, collectionStatus: status, mediaType: mediaType, rating: rating }, null);

		if (status == "Nothing") {
			$("#updateAlbumLink").hide();
			$("#addAlbumLink").show();
		} else {
			$("#addAlbumLink").hide();
			$("#updateAlbumLink").show();
		}

		vdb.ui.showSuccessMessage(vdb.resources.album.addedToCollection);

	}}]});

	var addAlbumLinkPos;
	if ($("#addAlbumLink").is(":visible"))
		addAlbumLinkPos = $("#addAlbumLink").offset();
	else
		addAlbumLinkPos = $("#updateAlbumLink").offset();

	$("#editCollectionDialog").dialog("option", "position", [addAlbumLinkPos.left, addAlbumLinkPos.top + 35]);

	$("#addAlbumLink").click(function () {

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	$("#updateAlbumLink").click(function () {

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	$("#removeAlbumLink").click(function () {

		$.post("../../User/RemoveAlbumFromUser", { albumId: albumId }, function (result) {

			$("#addAlbumLink").show();
			$("#removeAlbumLink").hide();

		});

		return false;

	});

	initReportEntryPopup(saveStr, hostAddress + "/Album/CreateReport", { albumId: albumId });

	$("#editTags").click(function () {

		$.get("../../Album/TagSelections", { albumId: albumId }, function (content) {

			$("#editTagsAlbumId").val(albumId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

		});

		return false;

	});

	$("td.artistList a").vdbArtistToolTip();

}