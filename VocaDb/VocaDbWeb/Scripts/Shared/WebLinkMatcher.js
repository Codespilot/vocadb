var vdb;
(function (vdb) {
    /// <reference path="../Models/WebLinkCategory.ts" />
    /// <reference path="../typings/underscore/underscore.d.ts" />
    (function (utils) {
        var c = vdb.models;

        var WebLinkMatcher = (function () {
            function WebLinkMatcher(url, desc, cat) {
                this.url = url;
                this.desc = desc;
                this.cat = cat;
            }
            WebLinkMatcher.matchWebLink = function (url) {
                return _.find(WebLinkMatcher.matchers, function (item) {
                    return (url.indexOf(item.url) != -1);
                });
            };
            WebLinkMatcher.matchers = [
                { url: "alice-books.com/", desc: "Alice Books", cat: c.WebLinkCategory.Commercial },
                { url: "www.amazon.co.jp/", desc: "Amazon", cat: c.WebLinkCategory.Commercial },
                { url: "www.amazon.com/", desc: "Amazon", cat: c.WebLinkCategory.Commercial },
                { url: "ameblo.jp/", desc: "Blog", cat: c.WebLinkCategory.Official },
                { url: "www.amiami.com/", desc: "AmiAmi", cat: c.WebLinkCategory.Commercial },
                { url: "www.animate-onlineshop.jp/", desc: "Animate Online Shop", cat: c.WebLinkCategory.Commercial },
                { url: "bilibili.tv/", desc: "Bilibili", cat: c.WebLinkCategory.Official },
                { url: "www.cdjapan.co.jp/detailview.html", desc: "CDJapan", cat: c.WebLinkCategory.Commercial },
                { url: "www.discogs.com/", desc: "Discogs", cat: c.WebLinkCategory.Reference },
                { url: "exittunes.com/", desc: "Exit Tunes", cat: c.WebLinkCategory.Official },
                { url: "www.facebook.com/", desc: "Facebook", cat: c.WebLinkCategory.Official },
                { url: ".fc2.com", desc: "Blog", cat: c.WebLinkCategory.Official },
                { url: "itunes.apple.com/", desc: "iTunes", cat: c.WebLinkCategory.Commercial },
                { url: "karent.jp/", desc: "KarenT", cat: c.WebLinkCategory.Commercial },
                { url: "last.fm/", desc: "Last.fm", cat: c.WebLinkCategory.Official },
                { url: "listen.jp/store/", desc: "Listen Japan", cat: c.WebLinkCategory.Commercial },
                { url: "shop.melonbooks.co.jp/", desc: "Melonbooks", cat: c.WebLinkCategory.Commercial },
                { url: "mikudb.com/", desc: "MikuDB", cat: c.WebLinkCategory.Reference },
                { url: "www5.atwiki.jp/hmiku/", desc: "MikuWiki", cat: c.WebLinkCategory.Reference },
                { url: "mora.jp/", desc: "mora", cat: c.WebLinkCategory.Commercial },
                { url: "chokuhan.nicovideo.jp/", desc: "NicoNico Chokuhan", cat: c.WebLinkCategory.Commercial },
                { url: "dic.nicovideo.jp/", desc: "NicoNicoPedia", cat: c.WebLinkCategory.Reference },
                { url: "nicovideo.jp/user/", desc: "NND Account", cat: c.WebLinkCategory.Official },
                { url: "com.nicovideo.jp/community/", desc: "NND Community", cat: c.WebLinkCategory.Official },
                { url: "nicovideo.jp/mylist/", desc: "NND MyList", cat: c.WebLinkCategory.Official },
                { url: "piapro.jp/", desc: "Piapro", cat: c.WebLinkCategory.Official },
                { url: "www.pixiv.net/member.php", desc: "Pixiv", cat: c.WebLinkCategory.Official },
                { url: "books.rakuten.co.jp/", desc: "Rakuten", cat: c.WebLinkCategory.Commercial },
                { url: "soundcloud.com/", desc: "SoundCloud", cat: c.WebLinkCategory.Official },
                { url: "toranoana.jp/mailorder/article/", desc: "Toranoana", cat: c.WebLinkCategory.Commercial },
                { url: "twitter.com/", desc: "Twitter", cat: c.WebLinkCategory.Official },
                { url: "www24.atwiki.jp/utauuuta/", desc: "UTAU wiki (JP)", cat: c.WebLinkCategory.Reference },
                { url: "vgmdb.net/", desc: "VGMdb", cat: c.WebLinkCategory.Reference },
                { url: "vimeo.com/", desc: "Vimeo", cat: c.WebLinkCategory.Official },
                { url: "vocaloiders.com/", desc: "Vocaloiders", cat: c.WebLinkCategory.Reference },
                { url: "vocaloid.wikia.com/wiki/", desc: "Vocaloid Wiki", cat: c.WebLinkCategory.Reference },
                { url: "www.yesasia.com/", desc: "YesAsia", cat: c.WebLinkCategory.Commercial },
                { url: "youtube.com/channel/", desc: "Youtube Channel", cat: c.WebLinkCategory.Official },
                { url: "youtube.com/user/", desc: "Youtube Channel", cat: c.WebLinkCategory.Official }
            ];
            return WebLinkMatcher;
        })();
        utils.WebLinkMatcher = WebLinkMatcher;
    })(vdb.utils || (vdb.utils = {}));
    var utils = vdb.utils;
})(vdb || (vdb = {}));
