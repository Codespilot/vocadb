
function CreateSongViewModel() {

	var self = this;

	this.Artists = ko.observableArray([]);
	this.DupeEntries = ko.observableArray([]);
	this.NameOriginal = ko.observable("");
	this.NameRomaji = ko.observable("");
	this.NameEnglish = ko.observable("");

	this.HasName = ko.computed(function() {
		return self.NameOriginal() || self.NameRomaji() || self.NameEnglish();
	});
	
	function checkDuplicatesAndPV() {
		
		var term1 = self.NameOriginal();
		var term2 = self.NameRomaji();
		var term3 = self.NameEnglish();
		var pv1 = $("#pv1").val();
		var pv2 = $("#pv2").val();

		$.post(vdb.functions.mapUrl("/Song/FindDuplicate"), { term1: term1, term2: term2, term3: term3, pv1: pv1, pv2: pv2, getPVInfo: true }, function (result) {

			self.DupeEntries(result.Matches);

			if (result.Title && !self.HasName()) {
				self.NameOriginal(result.Title);
			}

			if (result.Artists && !$("#artistsTableBody tr").length) {

				$(result.Artists).each(function () {
					$.post("../../Artist/CreateArtistContractRow", { artistId: this.Id }, function (row) {
						var artistsTable = $("#artistsTableBody");
						artistsTable.append(row);
						$("#artistsTableBody a.artistLink:last").vdbArtistToolTip();
					});
				});

			}

		});

	}

	// Note: we don't want to check right after value changes, only after focusing out.
	$("input.dupeField").focusout(checkDuplicatesAndPV);
	
	if ($("#pv1").val()) {
		checkDuplicatesAndPV();
	}

	function acceptArtistSelection(artistId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../Artist/CreateArtistContractRow", { artistId: artistId }, function (row) {
				var artistsTable = $("#artistsTableBody");
				artistsTable.append(row);
				$("#artistsTableBody a.artistLink:last").vdbArtistToolTip();
			});
		}

	}

	var artistAddList = $("#artistAddList");
	var artistAddName = $("input#artistAddName");
	var artistAddBtn = $("#artistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{
			allowCreateNew: false,
			acceptBtnElem: artistAddBtn,
			acceptSelection: acceptArtistSelection,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
			createOptionSecondRow: function (item) { return item.AdditionalNames; },
			extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,Producer,Circle,OtherGroup,Unknown,Animator,Illustrator,Lyricist,OtherIndividual" },
			height: 300
		});

	$("a.artistRemove").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

	$("#artistsTableBody a.artistLink").vdbArtistToolTip();

}

function initPage() {

	ko.applyBindings(new CreateSongViewModel());

}