
$(document).ready(function () {

	$("#globalSearchTerm").autocomplete({
		source: function (request, response) {

			var term = request.term;
			var entryType = $("#globalSearchBox input.globalSearchEntryType:checked").val();

			if (entryType == "Undefined") {
				$.post("../../Home/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Album") {
				$.post("../../Album/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Artist") {
				$.post("../../Artist/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Song") {
				$.post("../../Song/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			}

		},
		select: function (event, ui) {
			$("#globalSearchTerm").val(ui.item.value);
			$("#globalSearchBox").submit();
		}
	});


});

function entryFindCallback(response, results) {

	response($.map(results, function( item ) {
		return item; 
	}));

}