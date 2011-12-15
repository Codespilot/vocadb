
function initPage() {

	/*$("input.nameField").tooltip({

		position: "bottom center",

		// a little tweaking of the position
		offset: [5, 0],

		// use the built-in fadeIn/fadeOut effect
		effect: "fade",

		// custom opacity setting
		opacity: 0.8

	});*/

	$("input.nameField").focusout(function () {

		var term1 = $("#nameOriginal").val();
		var term2 = $("#nameRomaji").val();
		var term3 = $("#nameEnglish").val();

		$.post("../../Album/FindDuplicate", { term1: term1, term2: term2, term3: term3 }, function (result) {

			if (result != "Ok") {
				$("#duplicateEntryWarning").html(result);
				$("#duplicateEntryWarning").show();
			} else {
				$("#duplicateEntryWarning").hide();
			}

		});

	});

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (isNullOrWhiteSpace(findTerm)) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm }, function (results) {

			$(artistList).empty();

			$(results.Items).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

		});

	});

	$("input#artistAddName").bind("paste", function (e) {
		var elem = $(this);
		setTimeout(function () {
			$(elem).trigger("keyup");
		}, 0);
	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");
		var artistId = $(artistList).val();

		if (isNullOrWhiteSpace(findTerm) || artistId == "")
			return;

		$.post("../../Artist/CreateArtistContractRow", { artistId: artistId }, function (row) {

			var addRow = $("#artistRow_new");
			addRow.before(row);
			$("input#artistAddName").val("");
			$("#artistAddList").empty();

		});

	});

	$("input.artistRemove").live("click", function () {

		$(this).parent().parent().remove();

	});

}