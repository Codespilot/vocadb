
function initPage(albumId) {

	var albumList = $("#albumList");
	var albumName = $("input#albumName");
	var targetAlbumId = $("#targetAlbumId");

	initEntrySearch(albumName, albumList, "Album", "../../Album/FindJson",
		{
			idElem: targetAlbumId,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.ArtistString },
			createTitle: function (item) { return item.AdditionalNames }
		});	

	/*$("input#albumName").keyup(function () {

		var findTerm = $(this).val();
		var albumList = $("#albumList");

		if (findTerm.length == 0) {

			$(albumList).empty();
			return;

		}

		$.post("../../Album/FindJson", { term: findTerm }, function (results) {

			$(albumList).empty();

			$(results.Items).each(function () {

				if (this.Id != albumId) {
					addOption(albumList, this.Id, this.Name
						+ (this.AdditionalNames != "" ? " (" + this.AdditionalNames + ")" : ""));
				}

			});

		});

	});*/

	$("#mergeBtn").click(function () {

		var targetAlbumId = $("#albumList").val();

		if (targetAlbumId == null || targetAlbumId == "") {
			alert("Album must be selected!");
			return false;
		}

		return confirm("Are you sure you want to merge the albums?");

	});	

}