
function initPage(songId) {

	$("#addAlbumLink").button({ icons: { primary: 'ui-icon-heart'} });
	$("#removeAlbumLink").button({ icons: { primary: 'ui-icon-close'} });
	$("#editAlbumLink").button({ icons: { primary: 'ui-icon-wrench'} });	

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