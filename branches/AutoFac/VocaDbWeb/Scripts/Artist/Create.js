
function initPage() {

	$("input.dupeField").focusout(function () {

		var term1 = $("#nameOriginal").val();
		var term2 = $("#nameRomaji").val();
		var term3 = $("#nameEnglish").val();
		var linkUrl = $("#webLinkUrl").val();

		$.post("../../Artist/FindDuplicate", { term1: term1, term2: term2, term3: term3, linkUrl: linkUrl }, function (result) {

			if (result != "Ok") {
				$("#duplicateEntryWarning").html(result);
				$("#duplicateEntryWarning").show();
				$("#duplicateEntryWarning a").vdbArtistToolTip();
			} else {
				$("#duplicateEntryWarning").hide();
			}

		});

	});

	initWebLinksList();

}