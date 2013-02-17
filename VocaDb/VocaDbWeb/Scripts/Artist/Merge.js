
function initPage(artistId) {

	function acceptTargetArtist(targetArtistId) {

		$.post(vdb.functions.mapUrl("/Artist/Name"), { id: targetArtistId }, function (content) {
			$("#targetArtist").html("<a href='" + vdb.functions.mapUrl("/Artist/Details/" + targetArtistId) + "'>" + content + "</a>");
		});

	}

	var artistList = $("#artistList");
	var artistName = $("input#artistName");
	var targetArtistId = $("#targetArtistId");

	initEntrySearch(artistName, artistList, "Artist", "../../Artist/FindJson",
		{
			acceptSelection: acceptTargetArtist,
			filter: function (item) { return item.Id != artistId; },
			idElem: targetArtistId,
			createOptionFirstRow: function (item) { return item.Name; },
			createOptionSecondRow: function (item) { return item.AdditionalNames; }
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