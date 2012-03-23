﻿
function initDialog() {

	$("input.tagSelection").button();

	$("input#newTagName").autocomplete({
		source: "../../Tag/Find"
	});

	$("input#addNewTag").click(function () {

		var name = $("#newTagName").val();

		if (name == "")
			return false;

		$("#newTagName").val("")

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
	var artistId = $("#editTagsArtistId").val();

	$.post("../../Artist/TagSelections", { artistId: artistId, tagNames: tagNamesStr }, function (content) {

		$("#tagList").html(content);

	});

	$("#editTagsPopup").dialog("close");

}

function initPage(artistId) {

	$("#addToUserLink").button({ icons: { primary: 'ui-icon-heart'} });
	$("#removeFromUserLink").button({ icons: { primary: 'ui-icon-close'} });
	$("#editArtistLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#editTags").button({ icons: { primary: 'ui-icon-tag'} });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("select", 1);
		return false;
	});

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: { "Save": saveTagSelections} });

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

	$("#editTags").click(function () {

		$.get("../../Artist/TagSelections", { artistId: artistId }, function (content) {

			$("#editTagsArtistId").val(artistId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

		});

		return false;

	});

}