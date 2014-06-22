using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Tests.Service.ExtSites {

	[TestClass]
	public class AffiliateLinkGeneratorTests {

		private AffiliateLinkGenerator generator;
		private const string paAffId = "852809";

		[TestInitialize]
		public void SetUp() {
			generator = new AffiliateLinkGenerator(new VdbConfigManager());
		}

		[TestMethod]
		public void PlayAsia() {
			
			var input = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html";
			var expected = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=852809";

			var result = generator.GenerateAffiliateLink(input);

			Assert.AreEqual(expected, result, "Play-asia affiliate link matches");

		}

		[TestMethod]
		public void PlayAsia_ReplaceAffId() {
			
			var input = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=12345";
			var expected = "http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=852809";

			var result = generator.GenerateAffiliateLink(input);

			Assert.AreEqual(expected, result, "Play-asia affiliate link matches");

		}

	}


}
