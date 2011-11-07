
function initPage() {

	$("input.nameField").focusout(function () {

		var term1 = $("#nameOriginal").val();
		var term2 = $("#nameRomaji").val();
		var term3 = $("#nameEnglish").val();

		$.post("../../Song/FindDuplicate", { term1: term1, term2: term2, term3: term3 }, function (result) {

			if (result != "Ok") {
				$("#duplicateSongWarning").html(result);
				$("#duplicateSongWarning").show();
			} else {
				$("#duplicateSongWarning").hide();
			}

		});

	});

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm, artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer,Unknown" }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

		});

	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");
		var artistId = $(artistList).val();

		if (findTerm.length == 0 || artistId == "")
			return;

		$.post("../../Song/CreateArtistForSongRow", { artistId: artistId }, function (row) {

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