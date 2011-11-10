
function initPage(userId) {

	$("#tabs").tabs();

	$("#mySettingsLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#messagesLink").button({ icons: { primary: 'ui-icon-mail-closed'} });
	$("#composeMessageLink").button({ icons: { primary: 'ui-icon-mail-closed'} });
	$("#editUserLink").button({ icons: { primary: 'ui-icon-wrench'} });
	$("#disableUserLink").button({ icons: { primary: 'ui-icon-close'} });

	$("#disableUserLink").click(function () {

		return confirm("Are you sure you want to disable this user?");

	});

}