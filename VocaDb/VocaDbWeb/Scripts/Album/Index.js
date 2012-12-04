
$(document).ready(function () {
	$("#createLink").button({ disabled: $("#createLink").hasClass("disabled"), icons: { primary: 'ui-icon-plusthick' } });
	$("#manageDeletedLink").button({ icons: { primary: 'ui-icon-trash' } });
	$("#albums img").vdbAlbumToolTip();
});
