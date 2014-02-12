
function initPage(viewModel, albumId, discType) {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	//$(".helpToolTip").tooltip({ placement: "right", html: true });
	$("#pvLoader")
		.ajaxStart(function() { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#releaseEvent").autocomplete({
		source: "../../Album/FindReleaseEvents"
	});

	var editArtist;

	$("#editArtistRolesPopup").dialog({ autoOpen: false, width: 550, modal: true, buttons: [{ text: vdb.resources.shared.save, click: function () {

		var checkedRoles = $("#editArtistRolesPopup input.artistRoleCheck:checked").map(function () {
			return $(this).attr("id").split("_")[1];
		}).toArray();

		if (checkedRoles.length == 0)
			checkedRoles = ['Default'];

		var link = editArtist;
		if (link)
			link.rolesArray(checkedRoles);

		$("#editArtistRolesPopup").dialog("close");

	}}]});

	$("input.artistRoleCheck").button();

	$(document).on("click", "a.artistRolesEdit", function () {

		var data = ko.dataFor(this);

		editArtist = data;

		var roles = data.rolesArray();
		$("#editArtistRolesPopup input.artistRoleCheck").each(function () {
			$(this).removeAttr("checked");
			$(this).button("refresh");
		});

		$(roles).each(function () {
			var check = $("#editArtistRolesPopup #artistRole_" + this.trim());
			$(check).attr("checked", "checked");
			$(check).button("refresh");
		});

		$("#editArtistRolesPopup").dialog("open");

		return false;

	});

	initNamesList();

	$("#picAdd").click(function () {

		$.post("../../Shared/CreateEntryPictureFile", null, function (row) {

			$("#picturesTableBody").append(row);

		});

		return false;
		
	});

	$(document).on("click", "a.picRemove", function () {

		$(this).parent().parent().remove();
		return false;

	});

	$("#pvAdd").click(function () {

		var pvUrl = $("#pvUrl_new").val();

		$("#pvUrl_new").val("");

		$.post("../../Album/CreatePVForAlbumByUrl", { albumId: albumId, pvUrl: pvUrl }, function (response) {

			var result = handleGenericResponse(response);

			if (result == null)
				return;

			$("#pvTableBody").append(result);

		});

		return false;

	});

	$(document).on("click", "a.pvRemove", function () {

		$(this).parent().parent().remove();
		return false;

	});

}
