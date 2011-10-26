
function initPage(albumId) {

	$("input#albumName").keyup(function () {

		var findTerm = $(this).val();
		var albumList = $("#albumList");

		if (findTerm.length == 0) {

			$(albumList).empty();
			return;

		}

		$.post("../../Album/FindJson", { term: findTerm }, function (results) {

			$(albumList).empty();

			$(results).each(function () {

				if (this.Id != albumId) {
					addOption(albumList, this.Id, this.Name
						+ (this.AdditionalNames != "" ? " (" + this.AdditionalNames + ")" : ""));
				}

			});

		});

	});

	$("input:submit").click(function () {

		var targetAlbumId = $("#albumList").val();

		if (targetAlbumId == null || targetAlbumId == "") {
			alert("Album must be selected!");
			return false;
		}

		return confirm("Are you sure you want to merge the albums?");

	});	

}