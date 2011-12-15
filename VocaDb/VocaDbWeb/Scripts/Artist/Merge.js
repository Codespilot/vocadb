
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

			$(results.Items).each(function () {

				if (this.Id != artistId) {
					addOption(artistList, this.Id, this.Name 
						+ (this.AdditionalNames != "" ? " (" + this.AdditionalNames + ")" : ""));
				}

			});

		});

	});

	$("#mergeBtn").click(function () {

		var targetArtistId = $("#artistList").val();

		if (targetArtistId == null || targetArtistId == "") {
			alert("Artist must be selected!");
			return false;
		}

		return confirm("Are you sure you want to merge the artists?");

	});	
}