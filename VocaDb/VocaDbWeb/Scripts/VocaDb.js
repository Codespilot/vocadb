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
