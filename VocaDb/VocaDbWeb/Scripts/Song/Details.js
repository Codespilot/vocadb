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
	var songId = $("#editTagsSongId").val();

	$.post("../../Song/TagSelections", { songId: songId, tagNames: tagNamesStr }, function (content) {

		$("#tagList").html(content);

	});

	$("#editTagsPopup").dialog("close");

}

function initPage(songId) {

	$("#addFavoriteLink").button({ icons: { primary: 'ui-icon-heart'} });
	$("#removeFavoriteLink").button({ icons: { primary: 'ui-icon-close'} });
	$("#addToListLink").button({ icons: { primary: 'ui-icon-star'} });
	$("#editSongLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#editTags").button({ icons: { primary: 'ui-icon-tag'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: { "Save": saveTagSelections} });
	$("#addToListDialog").dialog({ autoOpen: false, width: 300, modal: false, buttons: { "Save": function () {

		$("#addToListDialog").dialog("close");

		var listId = $("#addtoListSelect").val();

		if (listId == null)
			return;

		$.post("../../Song/AddSongToList", { listId: listId, songId: songId }, null);		

	}}});

	var addToListLinkPos = $("#addToListLink").offset();
	$("#addToListDialog").dialog("option", "position", [addToListLinkPos.left, addToListLinkPos.top + 35]);

	$("#addFavoriteLink").click(function () {

		$.post("../../User/AddSongToFavorites", { songId: songId }, function (result) {

			$("#removeFavoriteLink").show();
			$("#addFavoriteLink").hide();

		});

		return false;

	});

	$("#removeFavoriteLink").click(function () {

		$.post("../../User/RemoveSongFromFavorites", { songId: songId }, function (result) {

			$("#addFavoriteLink").show();
			$("#removeFavoriteLink").hide();

		});

		return false;

	});

	$("#addToListLink").click(function () {

		$.get("../../Song/SongListsForUser", { ignoreSongId: songId }, function (lists) {

			var addtoListSelect = $("#addtoListSelect");
			addtoListSelect.html("");

			$(lists).each(function () {

				addOption(addtoListSelect, this.Id, this.Name);

			});

			$("#addToListDialog").dialog("open");

		});

		return false;

	});

	$("#editTags").click(function () {

		$.get("../../Song/TagSelections", { songId: songId }, function (content) {

			$("#editTagsSongId").val(songId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

		});

		return false;

	});

}