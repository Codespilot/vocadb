
function initPage(userId, loadingStr, confirmDisableStr) {

	$("#tabs").tabs({
		load: function (event, ui) {

			// Load only once
			$("#tabs").tabs("url", ui.index, "");
			$("#tabs").tabs("option", "spinner", loadingStr);

		}
	});

	$("#mySettingsLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#messagesLink").button({ icons: { primary: 'ui-icon-mail-closed'} });
	$("#composeMessageLink").button({ icons: { primary: 'ui-icon-mail-closed'} });
	$("#editUserLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#disableUserLink").button({ icons: { primary: 'ui-icon-close'} });

	$("#disableUserLink").click(function () {

		return confirm(confirmDisableStr);

	});

}