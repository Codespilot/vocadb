
function onChangeLang(event) {

	var select = event.target;

	var id = getId(select);
	var val = $(select).val();

	$.post("../../Song/EditNameLanguage", { nameId: id, nameLanguage: val });

}

function initPage(songId) {

	$("#tabs").tabs();

	$("input.nameEdit").live("change", function () {

		var id = getId(this);
		var val = $(this).val();

		$.post("../../Song/EditNameValue", { nameId: id, nameVal: val });

	});

	$("select.nameLanguage").change(onChangeLang);

	$("input.nameCopy").live("click", function () {

		var id = getId(this);
		var nameVal = $("input#nameEdit_" + id).val();
		var langId = $("select#nameLanguage_" + id).val();

		$("input#Name" + langId).val(nameVal);

	});

	$("input.nameDelete").live("click", function () {

		var id = getId(this);

		$.post("../../Song/DeleteName", { nameId: id });

		$("tr#nameRow_" + id).remove();

	});

	$("input#nameAdd").click(function () {

		var newNameVal = $("input#nameEdit_new").val();
		var newLangId = $("select#nameLanguage_new").val();

		$.post("../../Song/CreateName", { songId: songId, nameVal: newNameVal, language: newLangId }, function (row) {

			$("#nameRow_new").before(row);			
			$("input#nameEdit_new").val("");

		});

	});

	$("input#artistAddName").keyup(function () {

		var findTerm = $(this).val();
		var artistList = $("#artistAddList");

		if (findTerm.length < 3) {

			$(artistList).empty();

			if (findTerm.length > 0) {
				addOption(artistList, "", "Create new artist named '" + findTerm + "'");
			}

			return;
		}

		$.post("../../Artist/FindJson", { term: findTerm, artistTypes: "Performer,Producer" }, function (results) {

			$(artistList).empty();

			$(results).each(function () {

				addOption(artistList, this.Id, this.Name);

			});

			addOption(artistList, "", "Create new artist named '" + findTerm + "'");

		});

	});

	$("#artistAddBtn").click(function () {

		var findTerm = $("input#artistAddName").val();
		var artistList = $("#artistAddList");

		if (findTerm.length == 0)
			return;

		var artistId = $(artistList).val();

		if (artistId == "") {
			$.post("../../Song/AddNewArtist", { songId: songId, newArtistName: findTerm }, artistAdded);
		} else {
			$.post("../../Song/AddExistingArtist", { songId: songId, artistId: artistId }, artistAdded);
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
		$.post("../../Song/DeleteArtistForSong", { artistForSongId: id }, function () {

			$("tr#artistRow_" + id).remove();

		});

	});

	$("#pvAdd").click(function () {

		var service = $("#pvService_new").val();
		var pvUrl = $("#pvUrl_new").val();
		var pvType = $("#pvType_new").val();

		$("#pvUrl_new").val("");

		$.post("../../Song/CreatePVForSongByUrl", { songId: songId, pvUrl: pvUrl, type: pvType }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
				return;
			}

			var addRow = $("#pvRow_new");
			addRow.before(response.Result);

		});

	});

	$("input.pvRemove").live("click", function () {

		var id = getId(this);
		$.post("../../Song/DeletePVForSong", { pvForSongId: id }, function () {

			$("tr#pvRow_" + id).remove();

		});
		
	});

	$("#lyricsAdd").click(function () {

		var lang = $("#lyricsLanguage_new").val();
		var source = $("#lyricsSource_new").val();
		var value = $("#lyricsValue_new").val();

		$.post("../../Song/CreateLyrics", { }, function (html) {

			$("#lyricsAdd").before(html);

		});

	});

	$(".deleteLyrics").live("click", function () {

		var id = getId(this);
		$("#lyricsRow_" + id).remove();

	});

}