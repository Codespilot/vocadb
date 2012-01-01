
function initWebLinksList() {

	$("#webLinkAdd").click(function () {

		$.post("../../Shared/CreateNewWebLink", null, function (row) {

			$("#webLinksListBody").append(row);

		});

		return false;

	});

	$("a.webLinkDelete").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

}