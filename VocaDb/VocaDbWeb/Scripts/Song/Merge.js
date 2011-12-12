
function initPage(songId) {

	$("input#songName").keyup(function () {

		var findTerm = $(this).val();
		var songList = $("#songList");

		if (findTerm.length == 0) {

			$(songList).empty();
			return;

		}

		$.post("../../Song/FindJsonByName", { term: findTerm }, function (results) {

			$(songList).empty();

			$(results.Items).each(function () {

				if (this.Id != songId) {
					addOption(songList, this.Id, this.Name
						+ (this.AdditionalNames != "" ? " (" + this.AdditionalNames + ")" : "") 
						+ (this.ArtistString != "" ? " (by " + this.ArtistString + ")" : ""));
				}

			});

		});

	});

	$("input:submit").click(function () {

		return confirm("Are you sure you want to merge the songs?");

	});
}