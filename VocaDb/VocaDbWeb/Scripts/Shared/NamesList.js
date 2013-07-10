
function initNamesList() {

	$("#nameAdd").click(function () {

		$.post("../../Shared/CreateNewName", null, function (row) {

			$("#namesListBody").append(row);

		});

		return false;

	});

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