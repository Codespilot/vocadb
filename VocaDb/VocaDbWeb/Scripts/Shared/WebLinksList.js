
function initWebLinksList() {

	var linkMatchers = [
		{ url: "www.amazon.co.jp/", desc: "Amazon" },
		{ url: "karent.jp/", desc: "KarenT" },
		{ url: "mikudb.com/", desc: "MikuDB" },
		{ url: "www5.atwiki.jp/hmiku/pages/", desc: "MikuWiki" },
		{ url: "nicovideo.jp/user/", desc: "NND Account" },
		{ url: "nicovideo.jp/mylist/", desc: "NND MyList" },
		{ url: "piapro.jp/", desc: "Piapro" },
		{ url: "toranoana.jp/mailorder/article/", desc: "Toranoana" },
		{ url: "twitter.com/", desc: "Twitter" },
		{ url: "vocaloid.wikia.com/wiki/", desc: "Vocaloid Wiki" },
		{ url: "youtube.com/user/", desc: "Youtube Channel" }
	];

	$("#webLinkAdd").click(function () {

		$.post("../../Shared/CreateNewWebLink", null, function (row) {

			$("#webLinksListBody").append(row);

		});

		return false;

	});

	$("a.webLinkDelete").live("click", function () {

		$(this).parent().parent().remove();
		return false;

	});

	$("input.webLinkUrl").live("change", function () {

		var descBox = $(this).parent().parent().find("input.webLinkDescription");
		var url = $(this).val();

		if ($(descBox).val() == "") {

			var matcher = _.find(linkMatchers, function (item) { return (url.indexOf(item.url) != -1); });

			if (matcher != undefined)
				$(descBox).val(matcher.desc);

		}

	});

}