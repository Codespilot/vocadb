
function initPage(jsonModel, songId, saveStr, deleteCommentStr, hostAddress) {

    $("#ratingButtons").buttonset();
	$("#addFavoriteLink").button({ disabled: $("#addFavoriteLink").hasClass("disabled"), icons: { primary: 'ui-icon-heart'} });
	$("#addLikeLink").button({ disabled: $("#addLikeLink").hasClass("disabled"), icons: { primary: 'ui-icon-plus' } });
	$("#removeFavoriteLink").button({ disabled: $("#removeFavoriteLink").hasClass("disabled"), icons: { primary: 'ui-icon-close' } });
	$("#addToListLink").button({ disabled: $("#addToListLink").hasClass("disabled"), icons: { primary: 'ui-icon-star'} });
	$("#editSongLink").button({ disabled: $("#editSongLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#editTags").button({ disabled: $("#editTags").hasClass("disabled"), icons: { primary: 'ui-icon-tag'} });
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock' } });
	$("#viewCommentsLink").click(function () {
		var index = $('#tabs ul li:last').index();
		$("#tabs").tabs("option", "active", index);
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

	initReportEntryPopup(saveStr, hostAddress + "/Song/CreateReport", { songId: songId });

	$("#editTags").click(function () {

		$.get(hostAddress + "/Song/TagSelections", { songId: songId }, function (content) {

			$("#editTagsSongId").val(songId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

			var pvPlayer = $("#pvPlayer");
			if (pvPlayer.length) {
				var pvPlayerBottom = pvPlayer.offset().top + pvPlayer.outerHeight() - $(window).scrollTop();
				var centerTop = (($(window).height() - $("#editTagsPopup").outerHeight()) / 2);
				var left = (($(window).width() - $("#editTagsPopup").outerWidth()) / 2);
				var top = Math.max(centerTop, pvPlayerBottom);
				$("#editTagsPopup").dialog("option", "position", [left, top]);
			}

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

		function addTag(tagName) {

			if (isNullOrWhiteSpace(tagName))
				return;

			$("#newTagName").val("");

			if ($("#tagSelection_" + tagName).length) {
				$("#tagSelection_" + tagName).prop('checked', true);
				$("#tagSelection_" + tagName).button("refresh");
				return;
			}

			$.post(hostAddress + "/Tag/Create", { name: tagName }, function (response) {

				if (!response.Successful) {
					alert(response.Message);
				} else {
					$("#tagSelections").append(response.Result);
					$("input.tagSelection").button();
				}

			});

		}

		$("input#newTagName").autocomplete({
			source: hostAddress + "/Tag/Find",
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

		$.post(hostAddress + "/Song/TagSelections", { songId: songId, tagNames: tagNamesStr }, function (content) {

			$("#tagList").html(content);

		});

		$("#editTagsPopup").dialog("close");

	}

	function tabLoaded(event, ui) {

		vdb.functions.disableTabReload(ui.tab);

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

		$(document).on("click", "a.deleteComment", function () {

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

	$("td.artistList a").vdbArtistToolTip();
	$("#albumList a").vdbAlbumWithCoverToolTip();

}