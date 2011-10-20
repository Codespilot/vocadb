
function initPage(artistId) {

	$("input#artistName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistList");

		if (findTerm.length == 0) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				if (this.Id != artistId) {
					addOption(artistList, this.Id, this.Name + " (" + this.AdditionalNames + ")");
				}

			});

		});

	});

}