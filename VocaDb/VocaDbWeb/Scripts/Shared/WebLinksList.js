
function initWebLinksList() {

	var linkMatchers = [
		{ url: "www.amazon.co.jp/", desc: "Amazon" },
		{ url: "www.animate-onlineshop.jp/", desc: "Animate Online Shop" },
		{ url: "www.cdjapan.co.jp/detailview.html", desc: "CDJapan" },
		{ url: "www.facebook.com/", desc: "Facebook" },
		{ url: "karent.jp/", desc: "KarenT" },
		{ url: "shop.melonbooks.co.jp/", desc: "Melonbooks" },
		{ url: "mikudb.com/", desc: "MikuDB" },
		{ url: "www5.atwiki.jp/hmiku/pages/", desc: "MikuWiki" },
		{ url: "nicovideo.jp/user/", desc: "NND Account" },
		{ url: "nicovideo.jp/mylist/", desc: "NND MyList" },
		{ url: "piapro.jp/", desc: "Piapro" },
		{ url: "soundcloud.com/", desc: "SoundCloud" },
		{ url: "toranoana.jp/mailorder/article/", desc: "Toranoana" },
		{ url: "twitter.com/", desc: "Twitter" },
		{ url: "vgmdb.net/", desc: "VGMdb" },
		{ url: "vocaloid.wikia.com/wiki/", desc: "Vocaloid Wiki" },
		{ url: "www.yesasia.com/", desc: "YesAsia" },
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

        // TODO: this is quite a hack to support artist creation page. Should be done using knockoutjs to be honest.
	    if (descBox.length == 0)
	        descBox = $(this).parent().parent().parent().find("input.webLinkDescription");

		var url = $(this).val();

		if ($(descBox).val() == "") {

			var matcher = _.find(linkMatchers, function (item) { return (url.indexOf(item.url) != -1); });

			if (matcher != undefined)
				$(descBox).val(matcher.desc);

		}

	});

}