
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

	$("a.nameDelete").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

}