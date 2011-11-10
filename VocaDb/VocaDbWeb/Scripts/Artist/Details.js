
function initPage(albumId) {

	$("#editArtistLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });

	$("#tabs").tabs({
		load: function (event, ui) {

			if (ui.index == 1)
				tabLoaded("../../Artist", albumId, event, ui);

			// Load only once
			$("#tabs").tabs("url", ui.index, "");
			$("#tabs").tabs("option", "spinner", 'Loading...');

		}
	});

}