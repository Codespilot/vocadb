
function initPage(songId) {

	var source = new Array();

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

		if (findTerm.length == 0) {

			$(songList).empty();
			return;

		}

		$.post("../../Song/FindJsonByName", { term: findTerm }, function (results) {

			//$(songList).empty();
			var rows = new Array();

			$(results.Items).each(function () {


				if (this.Id != songId) {
					rows.push({ value: this.Id, html: "<div tabIndex=0 style='padding: 1px;'><input type='hidden' class='songId' value='" + this.Id + "' /><div>" + this.Name + "</div><div>" + this.ArtistString + "</div></div>", title: this.AdditionalNames });
					//$(songList).jqxListBox('insertAt', "<div tabIndex=0 style='padding: 1px;'><div>" + this.Name + "</div><div>" + this.ArtistString + "</div></div>", 1);
					/*addOption(songList, this.Id, this.Name
					+ (this.AdditionalNames != "" ? " (" + this.AdditionalNames + ")" : "") 
					+ (this.ArtistString != "" ? " (by " + this.ArtistString + ")" : ""));*/
				}

			});

			$(songList).jqxListBox({ source: rows });

		});

	});

	$("#mergeBtn").click(function () {

		return confirm("Are you sure you want to merge the songs?");

	});
}