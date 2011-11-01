
function initPage(albumId) {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();

	$("input.nameDelete").live("click", function () {

		$(this).parent().parent().remove();

	});

	$("input#nameAdd").click(function () {

		var newNameVal = $("input#nameEdit_new").val();
		var newLangId = $("select#nameLanguage_new").val();

		$.post("../../Shared/CreateName", { nameVal: newNameVal, language: newLangId }, function (row) {

			$("#nameRow_new").before(row);
			$("input#nameEdit_new").val("");

		});

	});

	$("input.webLinkDelete").live("click", function () {

		$(this).parent().parent().remove();

	});

	$("input#webLinkAdd").click(function () {

		var newDescription = $("input#webLinkDescription_new").val();
		var newUrl = $("input#webLinkUrl_new").val();

		$.post("../../Shared/CreateWebLink", { description: newDescription, url: newUrl }, function (row) {

			$("#webLinkRow_new").before(row);
			$("input#webLinkDescription_new").val("");
			$("input#webLinkUrl_new").val("");

		});

	});

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0) {

			$(artistList).empty();
			return;

		}

		$.post("../../Artist/FindJson", { term: findTerm }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

			//addOption(artistList, "", "Create new artist named '" + findTerm + "'");

		});

	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0)
			return;

		var artistId = $(artistList).val();

		if (artistId == "") {
			//$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: findTerm }, artistAdded);
		} else {
			$.post("../../Album/AddExistingArtist", { albumId: albumId, artistId: artistId }, artistAdded);
		}

	});

	function artistAdded(row) {

		var addRow = $("#artistRow_new");
		addRow.before(row);
		$("input#artistAddName").val("");
		$("#artistAddList").empty();

	}

	$("input.artistRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteArtistForAlbum", { artistForAlbumId: id }, function () {

			$("tr#artistRow_" + id).remove();

		});

	});

	$("input#songAddName").keyup(function () {

		var findTerm = $(this).val();
		var songList = $("#songAddList");

		if (findTerm.length == 0) {

			$(songList).empty();
			return;

		}

		$.post("../../Song/FindJsonByName", { term: findTerm }, function (results) {

			$(songList).empty();

			$(results.Items).each(function () {

				addOption(songList, this.Id, this.Name + (this.ArtistString != "" ? " (by " + this.ArtistString + ")" : ""));

			});

			addOption(songList, "", "Create new song named '" + findTerm + "'");

		});

	});

	$("#songAddBtn").click(function () {

		var findTerm = $("input#songAddName").val();
		var songList = $("#songAddList");

		if (findTerm.length == 0)
			return;

		var songId = $(songList).val();

		if (songId == "") {
			$.post("../../Album/AddNewSong", { albumId: albumId, newSongName: findTerm }, songAdded);
		} else {
			$.post("../../Album/AddExistingSong", { albumId: albumId, songId: songId }, songAdded);
		}

	});

	function songAdded(row) {

		var addRow = $("#songRow_new");
		addRow.before(row);
		$("input#songAddName").val("");
		$("#songAddList").empty();

	}

	$("input.songMoveUp").live("click", function () {

		var id = getId(this);
		$.post("../../Album/MoveSongInAlbumUp", { songInAlbumId: id }, songListChanged);

	});

	$("input.songMoveDown").live("click", function () {

		var id = getId(this);
		$.post("../../Album/MoveSongInAlbumDown", { songInAlbumId: id }, songListChanged);

	});

	$("input.songRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Album/DeleteSongInAlbum", { songInAlbumId: id }, function (songList) {

			$("tr#songRow_" + id).remove();
			songListChanged(songList);

		});

	});

	function songListChanged(songList) {

		for (var i = 0; i < songList.length; ++i) {

			var item = songList[i];

			var id = item.Id;
			var newTrackNum = item.TrackNumber;
			var trackNumElem = $("#songTrackNumber_" + id);
			var oldTrackNum = $(trackNumElem).val();

			if (newTrackNum != oldTrackNum) {

				var row = $("tr#songRow_" + id);

				$(trackNumElem).val(newTrackNum);

				var nextItem = (i < songList.length - 1 ? songList[i + 1] : null);
				var nextItemId = (nextItem != null ? nextItem.Id : null);
				var oldNextRow = $(row).next();
				var newNextRow = (nextItemId != null ? $("tr#songRow_" + nextItemId) : null);

				var nextRowId = getId(oldNextRow);

				if (nextItemId != nextRowId) {

					$(row).remove();

					if (newNextRow != null) {

						newNextRow.before(row);

					} else {

						var addRow = $("#songRow_new");
						addRow.before(row);

					}

				}

			}

		}

	}

}
