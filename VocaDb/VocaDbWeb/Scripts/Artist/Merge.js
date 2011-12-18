
function initPage(artistId) {

	var artistList = $("#artistList");
	var artistName = $("input#artistName");
	var targetArtistId = $("#targetArtistId");

	initEntrySearch(artistName, artistList, "Artist", "../../Artist/FindJson",
		{
			idElem: targetArtistId,
			createOptionFirstRow: function (item) { return item.Name },
			createOptionSecondRow: function (item) { return item.AdditionalNames }
		});

	$("#mergeBtn").click(function () {

		var targetArtistId = $("#targetArtistId").val();

		if (targetArtistId == null || targetArtistId == "") {
			alert("Artist must be selected!");
			return false;
		}

		return confirm("Are you sure you want to merge the artists?");

	});	
}