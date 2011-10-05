function addOption(select, value, text) {

	$(select).append("<option value=\"" + value + "\">" + text + "</option>");

}

function getId(elem) {
	var parts = $(elem).attr('id').split("_");
	return (parts.length >= 2 ? parts[1] : null);
}
