using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.ExtSites {

	/// <summary>
	/// Generates affiliate (paid) links to partner sites.
	/// </summary>
	public class AffiliateLinkGenerator {

		private static readonly RegexLinkMatcher cdjRegex = new RegexLinkMatcher("http://www.cdjapan.co.jp/aff/click.cgi/PytJTGW7Lok/4412/A585851/detailview.html?{0}",
			@"http://www.cdjapan\.co\.jp/detailview.html\?(KEY=\w+-\d+)");

		public static string GenerateAffiliateLink(string url) {

			if (!cdjRegex.IsMatch(url))
				return url;

			return cdjRegex.MakeLinkFromUrl(url);

		}

	}

}
