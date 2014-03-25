using System.Configuration;
using System.Linq;
using System.Xml;

namespace VocaDb.Model.Utils.Config {

	/*public class GlobalLinksSection : ConfigurationSectionGroup {

		public GlobalLinksSection() {
			BigBanners = new LinksSection();
			SmallBanners = new LinksSection();
			SocialLinks = new LinksSection();
		}

		[ConfigurationProperty("bigBanners")] 
		public LinksSection BigBanners { get; private set; }

		[ConfigurationProperty("smallBanners")] 
		public LinksSection SmallBanners { get; private set; }

		[ConfigurationProperty("socialLinks")] 
		public LinksSection SocialLinks { get; private set; }

	}

	public class LinksSection : ConfigurationSection {

		public LinkElement[] Links { get; set; }

	}

	public class LinkElement : ConfigurationElement {
		
		public string BannerImg {
			get { return (string)this["bannerImg"]; }
		}

		public string Title {
			get { return (string)this["title"]; }
		}

		public string Url {
			get { return (string)this["url"]; }
		}

	}*/

	/*public class GlobalLinksSection : ConfigurationSectionGroup {

		public LinksSection BigBanners {
			get {
				return (LinksSection)this.Sections["bigBanners"];
			}
		}

		public LinksSection SmallBanners {
			get {
				return (LinksSection)this.Sections["smallBanners"];
			}
		}

		public LinksSection SocialLinks {
			get {
				return (LinksSection)this.Sections["socialLinks"];
			}
		}

	}*/

	public class GlobalLinksSection : ConfigurationSectionGroup {

		public LinksSection BigBanners { get; set; }

		public LinksSection SmallBanners { get; set; }

		public LinksSection SocialSites { get; set; }

	}

	public class LinksSection : ConfigurationSection {

		public LinkElement[] Links { get; set; }

	}

	public class LinksSectionHandler : IConfigurationSectionHandler {

		public object Create(object parent, object configContext, XmlNode section) {
			
			var links = section.ChildNodes.Cast<XmlNode>().Select(n => new LinkElement(
				n.Attributes["bannerImg"].Value, n.Attributes["title"].Value, n.Attributes["url"].Value));

			return new LinksSection { Links = links.ToArray() };

		}

	}
	public class LinkElement  {

		public LinkElement(string bannerImg, string title, string url) {
			BannerImg = bannerImg;
			Title = title;
			Url = url;
		}

		public string BannerImg { get; set; }

		public string Title { get; set; }

		public string Url { get; set; }

	}

	/*public class LinkElement : ConfigurationElement {
		
		public string BannerImg {
			get { return (string)this["bannerImg"]; }
		}

		public string Title {
			get { return (string)this["title"]; }
		}

		public string Url {
			get { return (string)this["url"]; }
		}

	}*/

}
