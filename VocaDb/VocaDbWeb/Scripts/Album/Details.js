
function initPage(albumId) {

	$("#addAlbumLink").button({ icons: { primary: 'ui-icon-heart'} });
	$("#removeAlbumLink").button({ icons: { primary: 'ui-icon-close'} });
	$("#editAlbumLink").button({ icons: { primary: 'ui-icon-wrench'} });	

	$("#addAlbumLink").click(function () {

		$.post("../../User/AddExistingAlbum", { albumId: albumId }, function (result) {

			$("#removeAlbumLink").show();
			$("#addAlbumLink").hide();

		});

		return false;

	});

	$("#removeAlbumLink").click(function () {

		$.post("../../User/RemoveAlbumFromUser", { albumId: albumId }, function (result) {

			$("#addAlbumLink").show();
			$("#removeAlbumLink").hide();

		});

		return false;

	});

}