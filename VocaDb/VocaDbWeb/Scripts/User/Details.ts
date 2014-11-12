﻿import rep = vdb.repositories;

function initPage(confirmDisableStr: string) {

	$("#mySettingsLink").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#messagesLink").button({ icons: { primary: 'ui-icon-mail-closed' } });
	$("#composeMessageLink").button({ icons: { primary: 'ui-icon-mail-closed' } });
	$("#editUserLink").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#disableUserLink").button({ icons: { primary: 'ui-icon-close' } });
	$("#avatar").tooltip({ placement: "bottom" });

	$("#disableUserLink").click(function () {

		return confirm(confirmDisableStr);

	});

	$("#sfsCheckDialog").dialog({ autoOpen: false, model: true });
	$("#favoriteAlbums img").vdbAlbumToolTip();

}