
function initPage() {

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm, artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer" }, function (results) {

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

}