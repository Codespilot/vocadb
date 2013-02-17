
function initDialog() {

	function addTag(tagName) {

		if (isNullOrWhiteSpace(tagName))
			return;

		$("#newTagName").val("");

		if ($("#tagSelection_" + tagName).length) {
			$("#tagSelection_" + tagName).prop('checked', true);
			$("#tagSelection_" + tagName).button("refresh");
			return;
		}

		$.post("../../Tag/Create", { name: tagName }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
			} else {
				$("#tagSelections").append(response.Result);
				$("input.tagSelection").button();
			}

		});

	}

	$("input.tagSelection").button();

	$("input#newTagName").autocomplete({
		source: "../../Tag/Find",
		select: function (event, ui) { addTag(ui.item.label); return false; }
	});

	$("#addNewTag").click(function () {

		addTag($("#newTagName").val());
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
	var artistId = $("#editTagsArtistId").val();

	$.post("../../Artist/TagSelections", { artistId: artistId, tagNames: tagNamesStr }, function (content) {

		$("#tagList").html(content);

	});

	$("#editTagsPopup").dialog("close");

}

function initPage(artistId, saveStr, hostAddress) {

	$("#addToUserLink").button({ disabled: $("#addToUserLink").hasClass("disabled"), icons: { primary: 'ui-icon-heart'} });
	$("#removeFromUserLink").button({ disabled: $("#removeFromUserLink").hasClass("disabled"), icons: { primary: 'ui-icon-close'} });
	$("#editArtistLink").button({ disabled: $("#editArtistLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#editTags").button({ disabled: $("#editTags").hasClass("disabled"), icons: { primary: 'ui-icon-tag'} });
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("option", "active", 1);
		return false;
	});

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: [{ text: saveStr, click: saveTagSelections}] });

	$("#tabs").tabs({
		load: function (event, ui) {

			if (ui.index == 1)
				tabLoaded("../../Artist", artistId, event, ui);

			// Load only once
			$("#tabs").tabs("url", ui.index, "");
			$("#tabs").tabs("option", "spinner", 'Loading...');

		}
	});

	$("#addToUserLink").click(function () {

		$.post("../../User/AddArtistForUser", { artistId: artistId }, function (result) {

			$("#removeFromUserLink").show();
			$("#addToUserLink").hide();

		});

		return false;

	});

	$("#removeFromUserLink").click(function () {

		$.post("../../User/RemoveArtistFromUser", { artistId: artistId }, function (result) {

			$("#addToUserLink").show();
			$("#removeFromUserLink").hide();

		});

		return false;

	});

	initReportEntryPopup(saveStr, hostAddress + "/Artist/CreateReport", { artistId: artistId });

	$("#editTags").click(function () {

		$.get("../../Artist/TagSelections", { artistId: artistId }, function (content) {

			$("#editTagsArtistId").val(artistId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

		});

		return false;

	});

	$("#newAlbums img").vdbAlbumToolTip();
	$("#topAlbums img").vdbAlbumToolTip();
	$("#groups a").vdbArtistToolTip();

}