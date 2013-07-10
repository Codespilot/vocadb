
function initPage(viewModel, albumId, discType) {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$(".helpToolTip").qtip();
	$("#pvLoader")
		.ajaxStart(function() { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#releaseEvent").autocomplete({
		source: "../../Album/FindReleaseEvents"
	});

	$("#editArtistRolesPopup").dialog({ autoOpen: false, width: 550, modal: true, buttons: [{ text: vdb.resources.shared.save, click: function () {

		var artistId = $("#rolesArtistId").val();
		var checkedRoles = $("#editArtistRolesPopup input.artistRoleCheck:checked").map(function () {
			return $(this).attr("id").split("_")[1];
		}).toArray();

		var link = viewModel.getArtistLink(artistId);
		if (link)
			link.rolesArray(checkedRoles);

		$.ajax({
			type: "POST",
			url: "../../Album/UpdateArtistForAlbumRoles",
			dataType: "json",
			traditional: true,
			data: { artistForAlbumId: artistId, roles: checkedRoles }
		});

		$("#editArtistRolesPopup").dialog("close");

	}}]});

	$("input.artistRoleCheck").button();

	$(document).on("click", "a.artistRolesEdit", function () {

		var data = ko.dataFor(this);

		var id = data.id;
		$("#rolesArtistId").val(id);

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

	function acceptArtistSelection(artistId, term) {

		if (isNullOrWhiteSpace(artistId)) {
			$.post("../../Album/AddNewArtist", { albumId: albumId, newArtistName: term }, artistAdded);
		} else {
			$.post("../../Album/AddExistingArtist", { albumId: albumId, artistId: artistId }, artistAdded);
		}

	}

	var artistAddList = $("#artistAddList");
	var artistAddName = $("input#artistAddName");
	var artistAddBtn = $("#artistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{ 
			acceptBtnElem: artistAddBtn, 
			acceptSelection: acceptArtistSelection,
			createNewItem: vdb.resources.albumEdit.addExtraArtist,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
			createOptionSecondRow: function (item) { return item.AdditionalNames; }
		});

	function artistAdded(link) {

		viewModel.artistLinks.push(new vdb.viewModels.ArtistForAlbumEditViewModel(viewModel.repository, link));

	}

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
