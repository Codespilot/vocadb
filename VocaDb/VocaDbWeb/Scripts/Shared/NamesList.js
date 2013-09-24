
function initNamesList() {

	$("#nameAliasAdd").click(function () {

		$.post("../../Shared/CreateNewAlias", function (row) {

			$("#namesListBody").append(row);

		});

		return false;

	});

	$(document).on("click", "a.nameDelete", function () {

		$(this).parent().parent().remove();
		return false;

	});

}