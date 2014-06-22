using System.Text.RegularExpressions;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Service.ExtSites {

	/// <summary>
	/// Generates affiliate (paid) links to partner sites.
	/// </summary>
	public class AffiliateLinkGenerator {

		/*private static readonly RegexLinkMatcher cdjRegex = new RegexLinkMatcher("http://www.cdjapan.co.jp/aff/click.cgi/PytJTGW7Lok/4412/A585851/detailview.html?{0}",
			@"http://www.cdjapan\.co\.jp/detailview.html\?(KEY=\w+-\d+)");*/

		private readonly string amazonAffId;
		private readonly string paAffId;

		private string AddOrReplaceParam(string url, string param, string val) {
			
			var paramEq = param + "=";

			if (url.Contains(paramEq)) {
					
				return Regex.Replace(url, paramEq + @"(\d+)", string.Format(@"{0}{1}", paramEq, val));

			} else if (url.Contains("?")) {
				return string.Format("{0}&{1}{2}", url, paramEq, val);
			} else {
				return string.Format("{0}?{1}{2}", url, paramEq, val);
			}

		}

		private string ReplaceAmazonLink(string url) {
			
			if (string.IsNullOrEmpty(amazonAffId) || !(url.Contains("www.amazon.com/") || url.Contains("www.amazon.co.jp")))
				return url;

			return AddOrReplaceParam(url, "tag", amazonAffId);

		}

		private string ReplacePlayAsiaLink(string url) {
			
			if (string.IsNullOrEmpty(paAffId) || !url.Contains("www.play-asia.com/"))
				return url;

			return AddOrReplaceParam(url, "affiliate_id", paAffId);

		}

		public AffiliateLinkGenerator(VdbConfigManager configManager) {
			amazonAffId = configManager.Affiliates.AmazonAffiliateId;
			paAffId = configManager.Affiliates.PlayAsiaAffiliateId;
		}

		public string GenerateAffiliateLink(string url) {

			if (string.IsNullOrEmpty(url))
				return url;

			/*if (cdjRegex.IsMatch(url)) {
				return cdjRegex.MakeLinkFromUrl(url);				
			}*/

			url = ReplacePlayAsiaLink(url);
			url = ReplaceAmazonLink(url);

			return url;


		}

	}

}
