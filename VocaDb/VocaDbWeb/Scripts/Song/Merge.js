
function initPage(artistId) {

	$("input#songName").keyup(function () {

		var findTerm = $(this).val();
		var songList = $("#songList");

		if (findTerm.length == 0) {

			$(songList).empty();
			return;

		}

		$.post("../../Song/FindJson", { term: findTerm }, function (results) {

			$(songList).empty();

			$(results).each(function () {

				if (this.Id != songId) {
					addOption(songList, this.Id, this.Name
						+ (this.AdditionalNames != "" ? " (" + this.AdditionalNames + ")" : ""));
				}

			});

		});

	});

	$("input:submit").click(function () {

		return confirm("Are you sure you want to merge the songs?");

	});
}