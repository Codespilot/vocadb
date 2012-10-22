﻿
function initWebLinksList() {

	var linkMatchers = [
		{ url: "www.amazon.co.jp/", desc: "Amazon", cat: "Commercial" },
		{ url: "www.amazon.com/", desc: "Amazon", cat: "Commercial" },
		{ url: "ameblo.jp/", desc: "Blog", cat: "Official" },
		{ url: "www.animate-onlineshop.jp/", desc: "Animate Online Shop", cat: "Commercial" },
		{ url: "www.cdjapan.co.jp/detailview.html", desc: "CDJapan", cat: "Commercial" },
		{ url: "www.discogs.com/", desc: "Discogs", cat: "Reference" },
		{ url: "www.facebook.com/", desc: "Facebook", cat: "Official" },
		{ url: "itunes.apple.com/", desc: "iTunes", cat: "Commercial" },
		{ url: "karent.jp/", desc: "KarenT", cat: "Commercial" },
		{ url: "listen.jp/store/", desc: "Listen Japan", cat: "Commercial" },
		{ url: "shop.melonbooks.co.jp/", desc: "Melonbooks", cat: "Commercial" },
		{ url: "mikudb.com/", desc: "MikuDB", cat: "Reference" },
		{ url: "www5.atwiki.jp/hmiku/pages/", desc: "MikuWiki", cat: "Reference" },
		{ url: "nicovideo.jp/user/", desc: "NND Account", cat: "Official" },
        { url: "com.nicovideo.jp/community/", desc: "NND Community", cat: "Official" },
		{ url: "nicovideo.jp/mylist/", desc: "NND MyList", cat: "Official" },
		{ url: "piapro.jp/", desc: "Piapro", cat: "Official" },
		{ url: "www.pixiv.net/member.php", desc: "Pixiv", cat: "Official" },
		{ url: "books.rakuten.co.jp/", desc: "Rakuten", cat: "Commercial" },
		{ url: "soundcloud.com/", desc: "SoundCloud", cat: "Official" },
		{ url: "toranoana.jp/mailorder/article/", desc: "Toranoana", cat: "Commercial" },
		{ url: "twitter.com/", desc: "Twitter", cat: "Official" },
		{ url: "vgmdb.net/", desc: "VGMdb", cat: "Reference" },
		{ url: "vimeo.com/", desc: "Vimeo", cat: "Official" },
		{ url: "vocaloid.wikia.com/wiki/", desc: "Vocaloid Wiki", cat: "Reference" },
		{ url: "www.yesasia.com/", desc: "YesAsia", cat: "Commercial" },
	    { url: "youtube.com/user/", desc: "Youtube Channel", cat: "Official" }
	];

	var linkDescriptions = _.map(linkMatchers, function (matcher) { return matcher.desc; });

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

	$("input.webLinkDescription").autocomplete({ source: linkDescriptions, delay: 0 });

	$("input.webLinkUrl").live("change", function () {

	    var descBox = $(this).parent().parent().find("input.webLinkDescription");

        // TODO: this is quite a hack to support artist creation page. Should be done using knockoutjs to be honest.
	    if (descBox.length == 0)
	        descBox = $(this).parent().parent().parent().find("input.webLinkDescription");

	    var catBox = $(this).parent().parent().find("select.webLinkCategory");

		var url = $(this).val();

		if ($(descBox).val() == "") {

			var matcher = _.find(linkMatchers, function (item) { return (url.indexOf(item.url) != -1); });

			if (matcher != undefined) {

			    $(descBox).val(matcher.desc);

			    if (catBox)
			        $(catBox).val(matcher.cat);

			}

		}

	});

}