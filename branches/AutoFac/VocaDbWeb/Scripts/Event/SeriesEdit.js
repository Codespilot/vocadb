
function initPage() {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });

	$("#createAliasLink").click(function () {

		var alias = $("#newAliasName").val();

		if (isNullOrWhiteSpace(alias)) {
			alert("Alias cannot be empty");
			return false;
		}

		$.post("../../Event/AliasForSeries", { name: alias }, function (row) {
			$("#aliases").append(row);
		});

		return false;

	});

	$("a.aliasRemove").live("click", function () {

		$(this).parent().remove();

		return false;

	});


}