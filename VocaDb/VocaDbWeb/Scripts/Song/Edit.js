
function initPage(songId) {

	var editArtist;
	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$("#pvLoader")
		.ajaxStart(function () { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#editArtistRolesPopup").dialog({ autoOpen: false, width: 550, modal: true, buttons: { "Save": function () {

		var checkedRoles = $("#editArtistRolesPopup input.artistRoleCheck:checked").map(function () {
			return $(this).attr("id").split("_")[1];
		}).toArray();

		if (checkedRoles.length == 0)
			checkedRoles = ['Default'];

		var link = editArtist;
		if (link)
			link.rolesArray(checkedRoles);

		$("#editArtistRolesPopup").dialog("close");

	}}});

	$("input.artistRoleCheck").button();

	$("#changeOriginalBtn").click(function () {

		$("#changeOriginalPanel").show();
		return false;

	});

	$("#clearOriginalBtn").click(function () {

	    $.post("../../Song/CreateSongLink", function (content) {
	        $("#originalContent").html(content);
	    });

	});

	function acceptOriginalSong(songId, term) {

		$("#changeOriginalPanel").hide();
		var newOriginalId = $("#changeOriginalId").val();

		$.post("../../Song/CreateSongLink", { songId: newOriginalId }, function (content) {
			$("#originalContent").html(content);
		});

	}

	var acceptNewOriginalBtn = $("#acceptNewOriginalBtn");
	var changeOriginalList = $("#changeOriginalList");
	var changeOriginalName = $("input#changeOriginalName");
	var changeOriginalId = $("#changeOriginalId");

	initEntrySearch(changeOriginalName, changeOriginalList, "Song", "../../Song/FindJsonByName",
		{
			acceptBtnElem: acceptNewOriginalBtn,
			acceptSelection: acceptOriginalSong,
			idElem: changeOriginalId,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.SongType + ")"; },
			createOptionSecondRow: function (item) { return item.ArtistString; },
			createTitle: function (item) { return item.AdditionalNames; },
			extraQueryParams: {
				songTypes: "Unspecified,Original,Remaster,Remix,Cover,Mashup,DramaPV,Other"
			},
			filter: function (item) { return item.Id != songId; },
			height: 250
		});

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

	$("#lyricsAdd").click(function () {

		$.post("../../Song/CreateLyrics", null, function (html) {

			$("#lyricsAdd").before(html);

		});

		return false;

	});

	$(document).on("click", "a.deleteLyrics", function () {

		$(this).parent().remove();
		return false;

	});

	$("#artistsTableBody a.artistLink").vdbArtistToolTip();
	
}
