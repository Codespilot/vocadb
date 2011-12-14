
function initPage(songId) {

	$("#songList").jqxListBox({ width: '400', height: '350' });

	$('#songList').bind('select', function (event) {

		var item = $("#songList").jqxListBox('getItem', args.index);

		if (item != null) {
			$("#targetSongId").val(item.value);
		}

	});

	$("input#songName").keyup(function () {

		var findTerm = $(this).val();
		var songList = $("#songList");

		if (isNullOrWhiteSpace(findTerm)) {

			$(songList).jqxListBox({ source: new Array() });
			return;

		}

		$.post("../../Song/FindJsonByName", { term: findTerm }, function (results) {

			if ($("input#songName").val() != results.Term)
				return;

			//$(songList).empty();
			var rows = new Array();

			$(results.Items).each(function () {

				if (this.Id != songId) {
					rows.push({ value: this.Id, html: "<div tabIndex=0 style='padding: 1px;'><input type='hidden' class='songId' value='" + this.Id + "' /><div>" + this.Name + "</div><div>" + this.ArtistString + "</div></div>", title: this.AdditionalNames });
				}

			});

			$(songList).jqxListBox({ source: rows });

		});

	});

	$("#mergeBtn").click(function () {

		return confirm("Are you sure you want to merge the songs?");

	});
}