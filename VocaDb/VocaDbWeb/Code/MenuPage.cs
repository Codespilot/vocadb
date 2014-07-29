﻿using System.Linq;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code {

	public abstract class MenuPage : VocaDbPage {

		public class Link {

			public Link(string title, string url, string bannerImg) {
				BannerImg = bannerImg;
				Title = title;
				Url = url;
			}

			public string BannerImg { get; set; }

			public string Title { get; set; }

			public string Url { get; set; }

		}

		static MenuPage() {
			
			var config = AppConfig.GetLobalLinksSection();

			if (config == null || config.AppLinks == null) {

				AppLinks = new[] {
					new Link("Google Play Store", "https://play.google.com/store/apps/details?id=com.coolappz.Vocadb", "en_app_rgb_wo_45.png"),
				};

			} else {
			
				AppLinks = config.AppLinks.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray();
	
			}

			if (config == null || config.BigBanners == null) {

				BigBanners = new [] {
					new Link("UtaiteDB", "http://utaitedb.net", "utaitedb.png"), 
					new Link("Virtual Emotions", "http://virtual-emotions.net", "VirtualEmotions.png"), 
					new Link("Project DIVA wiki", "http://projectdiva.wikispaces.com/", "pjd-wiki.png"),
					new Link("Mikufan.com", "http://www.mikufan.com", "mikufan_bannertest.png"),
					new Link("Vocaloidism", "http://vocaloidism.com", "vocaloidism-180px.png")			
				};

			} else {
				
				BigBanners = config.BigBanners.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray();

			}

			if (config == null || config.SmallBanners == null) {

				SmallBanners = new[] {
					new Link("VocaloidOtaku", "http://vocaloidotaku.net", "vo_small.png"),
					new Link("r/vocaloid", "http://www.reddit.com/r/vocaloid", "rvocaloid_small2.png")				
				};

			} else {
				
				SmallBanners = config.SmallBanners.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray();

			}

			if (config == null || config.SocialSites == null) {

				SocialLinks = new[] {
					new Link("Facebook", "https://www.facebook.com/vocadb", "facebook.png"),
					new Link("Twitter", "https://twitter.com/VocaDB", "Twitter_Logo.png"),
					new Link("Google+", "https://plus.google.com/112203842561345720098", "googleplus-icon.png")				
				};

			} else {
			
				SocialLinks = config.SocialSites.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray();
	
			}

		}

		public static Link[] AppLinks { get; private set; }

		public static Link[] BigBanners { get; private set; }

		public string BlogUrl {
			get {
				return UrlHelper.MakeLink(Config.SiteSettings.BlogUrl);
			}
		}

		public static Link[] SmallBanners { get; private set; }

		public static Link[] SocialLinks { get; private set; }

	}

}