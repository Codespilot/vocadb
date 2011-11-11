
function initPage(songId) {

	$("#addFavoriteLink").button({ icons: { primary: 'ui-icon-heart'} });
	$("#removeFavoriteLink").button({ icons: { primary: 'ui-icon-close'} });
	$("#editAlbumLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#lyricsAccordion").accordion({
		active: false,
		autoHeight: false,
		collapsible: true
	});

	$("#addFavoriteLink").click(function () {

		$.post("../../User/AddSongToFavorites", { songId: songId }, function (result) {

			$("#removeFavoriteLink").show();
			$("#addFavoriteLink").hide();

		});

		return false;

	});

	$("#removeFavoriteLink").click(function () {

		$.post("../../User/RemoveSongFromFavorites", { songId: songId }, function (result) {

			$("#addFavoriteLink").show();
			$("#removeFavoriteLink").hide();

		});

		return false;

	});
	
}