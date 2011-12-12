
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

	$("#editArtistLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#editTags").button({ icons: { primary: 'ui-icon-tag'} });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("select", 1);
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