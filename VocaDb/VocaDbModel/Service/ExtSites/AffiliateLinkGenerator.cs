using System;
using System.Text.RegularExpressions;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.ExtSites {

	/// <summary>
	/// Generates affiliate (paid) links to partner sites.
	/// </summary>
	public class AffiliateLinkGenerator {

		private static readonly RegexLinkMatcher cdjRegex = new RegexLinkMatcher("http://www.cdjapan.co.jp/aff/click.cgi/PytJTGW7Lok/4412/A585851/detailview.html?{0}",
			@"http://www.cdjapan\.co\.jp/detailview.html\?(KEY=\w+-\d+)");

		private const string paAffId = "852809";

		private string ReplacePlayAsiaLink(string url) {
			
			if (url.Contains("www.play-asia.com/")) {

				if (url.Contains("affiliate_id=")) {
					
					return Regex.Replace(url, @"affiliate_id=(\d+)", string.Format(@"affiliate_id={0}", paAffId));

				} else if (url.Contains("?")) {
					return string.Format("{0}&affiliate_id={1}", url, paAffId);
				} else {
					return string.Format("{0}?affiliate_id={1}", url, paAffId);
				}

			}

			return url;

		}

		public string GenerateAffiliateLink(string url) {

			if (string.IsNullOrEmpty(url))
				return url;

			/*if (cdjRegex.IsMatch(url)) {
				return cdjRegex.MakeLinkFromUrl(url);				
			}*/

			url = ReplacePlayAsiaLink(url);

			return url;


		}

	}

}
