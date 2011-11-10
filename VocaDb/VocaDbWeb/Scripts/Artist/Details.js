
function initPage(albumId) {

	$("#editArtistLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });

	$("#tabs").tabs({
		load: function (event, ui) {
			tabLoaded("../../Artist", albumId, event, ui);
		}
	});

}