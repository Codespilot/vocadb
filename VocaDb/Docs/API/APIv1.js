function byName(query, lang, start, maxResults, nameMatchMode) {

    jQuery.get(host + "/Api/v1/Song/ByName?query=" + query + "&lang=" + lang + "&start=" + start + "&maxResults=" + maxResults + "&nameMatchMode=" + nameMatchMode + "&format=JSON", null, callback, "jsonp");

}

function byPV(host, service, pvId, callback) {

    jQuery.get(host + "/Api/v1/Song/ByPV?service=" + service + "&pvId=" + pvId + "&format=JSON", null, callback, "jsonp");

}