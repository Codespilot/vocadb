function addOption(select, value, text) {

	$(select).append("<option value=\"" + value + "\">" + text + "</option>");

}

function getId(elem) {
	return $(elem).attr('id').split("_")[1];
}
