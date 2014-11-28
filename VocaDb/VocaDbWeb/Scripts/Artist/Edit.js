
function initPage(artistId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();

	function acceptGroupSelection(groupId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../Artist/AddCircle", { artistId: artistId, circleId: groupId }, function (row) {

				$("#groupTableBody").append(row);

			});
		}

	}

	var groupAddList = $("#groupAddList");
	var groupAddName = $("input#groupAddName");
	var groupAddBtn = $("#groupAddAcceptBtn");

	initEntrySearch(groupAddName, groupAddList, "Artist", "../../Artist/FindJson",
		{
			acceptBtnElem: groupAddBtn,
			acceptSelection: acceptGroupSelection,
			createOptionFirstRow: function (item) { return item.Name; },
			createOptionSecondRow: function (item) { return item.AdditionalNames; },
			extraQueryParams: { artistTypes: "Label,Circle,OtherGroup,Band" },
			height: 200,
			width: 350
		});

	$(document).on("click", "a.groupRemove", function () {

		$(this).parent().parent().remove();		
		return false;

	});

}
