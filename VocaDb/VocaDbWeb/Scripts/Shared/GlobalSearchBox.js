
$(document).ready(function () {

	$("#globalSearchTerm").autocomplete({
		source: function (request, response) {

			var term = request.term;
			var entryType = $("#globalSearchEntryType").val();

			if (entryType == "Album") {
				$.post("../../Album/FindJson", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Artist") {
				$.post("../../Artist/FindJson", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Song") {
				$.post("../../Song/FindJsonByName", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			}

		}
	});


});

function entryFindCallback(response, results) {

	$.map(results.Items, function( item ) {
		return item.Name; 
	});

}