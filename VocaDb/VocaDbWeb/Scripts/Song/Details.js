﻿
function initPage(songId, saveStr, deleteCommentStr, hostAddress) {

	$("#addFavoriteLink").button({ disabled: $("#addFavoriteLink").hasClass("disabled"), icons: { primary: 'ui-icon-heart'} });
	$("#removeFavoriteLink").button({ disabled: $("#removeFavoriteLink").hasClass("disabled"), icons: { primary: 'ui-icon-close'} });
	$("#addToListLink").button({ disabled: $("#addToListLink").hasClass("disabled"), icons: { primary: 'ui-icon-star'} });
	$("#editSongLink").button({ disabled: $("#editSongLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#editTags").button({ disabled: $("#editTags").hasClass("disabled"), icons: { primary: 'ui-icon-tag'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("select", 1);
		return false;
	});

	$("#tabs").tabs({
		load: function (event, ui) {
			tabLoaded(event, ui);
		}
	});

	$("#pvLoader")
		.ajaxStart(function () { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: [{ text: saveStr, click: saveTagSelections}] });
	$("#addToListDialog").dialog({ autoOpen: false, width: 300, modal: false, buttons: [{ text: saveStr, click: function () {

		$("#addToListDialog").dialog("close");

		var listId = $("#addtoListSelect").val();

		if (listId == null)
			return;

		$.post(hostAddress + "/Song/AddSongToList", { listId: listId, songId: songId }, null);		

	}}]});


	$("#reportSongPopup").dialog({ autoOpen: false, width: 300, modal: false, buttons: [{ text: saveStr, click: function () {

		$("#reportSongPopup").dialog("close");

		var reportType = $("#reportType").val();
		var notes = $("#reportNotes").val();

		$.post(hostAddress + "/Song/CreateReport", { songId: songId, reportType: reportType, notes: notes });

	}}]});


	$("#addFavoriteLink").click(function () {

		$.post(hostAddress + "/User/AddSongToFavorites", { songId: songId }, function (result) {

			$("#removeFavoriteLink").show();
			$("#addFavoriteLink").hide();

		});

		return false;

	});

	$("#removeFavoriteLink").click(function () {

		$.post(hostAddress + "/User/RemoveSongFromFavorites", { songId: songId }, function (result) {

			$("#addFavoriteLink").show();
			$("#removeFavoriteLink").hide();

		});

		return false;

	});

	$("#addToListLink").click(function () {

		$.get(hostAddress + "/Song/SongListsForUser", { ignoreSongId: songId }, function (lists) {

			var addToListLinkPos = $("#addToListLink").offset();
			if (addToListLinkPos != null) {
				$("#addToListDialog").dialog("option", "position", [addToListLinkPos.left, addToListLinkPos.top - $(window).scrollTop() + 35]);
			}

			var addtoListSelect = $("#addtoListSelect");
			addtoListSelect.html("");

			$(lists).each(function () {

				addOption(addtoListSelect, this.Id, this.Name);

			});

			$("#addToListDialog").dialog("open");

		});

		return false;

	});

	$("#reportEntryLink").click(function () {

		var addToListLinkPos = $("#reportEntryLink").offset();
		if (addToListLinkPos != null) {
			$("#reportSongPopup").dialog("option", "position", [addToListLinkPos.left, addToListLinkPos.top - $(window).scrollTop() + 35]);
		}

		$("#reportSongPopup").dialog("open");
		return false;

	});

	$("#editTags").click(function () {

		$.get(hostAddress + "/Song/TagSelections", { songId: songId }, function (content) {

			$("#editTagsSongId").val(songId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

			var pvPlayer = $("#pvPlayer");
			//var pvPlayerTop = pvPlayer.offset().top;
			//var pvPlayerHeight = pvPlayer.outerHeight();
			var pvPlayerBottom = pvPlayer.offset().top + pvPlayer.outerHeight() - $(window).scrollTop();
			var centerTop = (($(window).height() - $("#editTagsPopup").outerHeight()) / 2);
			var left = (($(window).width() - $("#editTagsPopup").outerWidth()) / 2);
			var top = Math.max(centerTop, pvPlayerBottom);
			$("#editTagsPopup").dialog("option", "position", [left, top]);

		});

		return false;

	});

	$(".pvLink").click(function () {

		var id = getId(this);
		$.post(hostAddress + "/Song/PVForSong", { pvId: id }, function (content) {
			$("#pvPlayer").html(content);
		});

		return false;

	});

	function initDialog() {

		$("input.tagSelection").button();

		$("input#newTagName").autocomplete({
			source: hostAddress + "/Tag/Find"
		});

		$("input#addNewTag").click(function () {

			var name = $("#newTagName").val();

			if (name == "")
				return false;

			$("#newTagName").val("");

			$.post(hostAddress + "/Tag/Create", { name: name }, function (response) {

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

		$.post(hostAddress + "/Song/TagSelections", { songId: songId, tagNames: tagNamesStr }, function (content) {

			$("#tagList").html(content);

		});

		$("#editTagsPopup").dialog("close");

	}

	function tabLoaded(event, ui) {

		$("#tabs").tabs("url", 1, "");

		$("#createComment").click(function () {

			var message = $("#newCommentMessage").val();

			if (message == "") {
				alert("Message cannot be empty");
				return false;
			}

			$("#newCommentMessage").val("");

			$.post(hostAddress + "/Song/CreateComment", { songId: songId, message: message }, function (result) {

				$("#newCommentPanel").after(result);

			});

			return false;

		});

		$("a.deleteComment").live("click", function () {

			if (!confirm(deleteCommentStr))
				return false;

			var btn = this;
			var id = getId(this);

			$.post(hostAddress + "/Song/DeleteComment", { commentId: id }, function () {

				$(btn).parent().parent().remove();

			});

			return false;

		});

	}

}