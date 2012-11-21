function addOption(select, value, text) {

	$(select).append("<option value=\"" + value + "\">" + text + "</option>");

}

function formatSongName(songContract) {

	return (songContract.Name + (songContract.ArtistString != "" ? " (by " + songContract.ArtistString + ")" : ""));

}

function getId(elem) {

	if ($(elem) == null || $(elem).attr('id') == null)
		return null;

	var parts = $(elem).attr('id').split("_");
	return (parts.length >= 2 ? parts[1] : null);
}

// Handles GenericResponse, displaying an error message if appropriate.
// Returns: null if the request FAILED, or the response content if the request succeeded.
function handleGenericResponse(response) {

	if (!response.Successful) {

		if (response.Message != null)
			alert(response.Message);

		return null;

	}

	return response.Result;

}

function htmlDecode(value) {
	if (value) {
		return $('<div />').html(value).text();
	} else {
		return '';
	}
}

function isNullOrWhiteSpace(str) {

	if (str == null || str.length == 0)
		return true;

	return !(/\S/.test(str));

}

String.prototype.trim = function () {
    return this.replace(/^\s*/, "").replace(/\s*$/, "");
};

vdb = {};
vdb.functions = vdb.functions || {};
vdb.values = vdb.values || {};
vdb.functions.mapUrl = function (relative) {
    return vdb.values.hostAddress + relative;
};

(function ($) {
    $.fn.vdbArtistToolTip = function () {

        this.each(function () {
            var elem = this;

            $(elem).qtip({
                content: {
                    text: 'Loading...',
                    ajax: {
                        url: vdb.functions.mapUrl('/Artist/PopupContent'),
                        type: 'GET',
                        data: { id: $(elem).data("entryId") }
                    }
                },
                position: {
                    viewport: $(window)
                },
                style: {
                    classes: "tooltip-wide"
                }
            });
        });

    };
})(jQuery);

(function ($) {
    $.fn.vdbAlbumToolTip = function () {

        this.each(function () {
            var elem = this;

            $(elem).qtip({
                content: {
                    text: 'Loading...',
                    ajax: {
                        url: vdb.functions.mapUrl('/Album/PopupContent'),
                        type: 'GET',
                        data: { id: $(elem).data("entryId") }
                    }
                },
                position: {
                    viewport: $(window)
                }
            });
        });

    };
})(jQuery);