using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices {

	[TestClass]
	public class VideoServiceFileTests {

		private VideoServiceFile videoService;

		private void TestGetVideoTitle(string url, string expected) {
			Assert.AreEqual(expected, videoService.GetVideoTitle(url).Title);			
		}

		private void TestIsValidFor(string url, bool expected) {
			Assert.AreEqual(expected, videoService.IsValidFor(url));
		}

		[TestInitialize]
		public void SetUp() {
			videoService = new VideoServiceFile();
		}

		[TestMethod]
		public void GetVideoTitle_Simple() {
			
			TestGetVideoTitle("http://caress.airtv.org/audio/car.ess - 2 UFO'r'IA.mp3", "car.ess - 2 UFO'r'IA.mp3");

		}

		[TestMethod]
		public void GetVideoTitle_WithParam() {
			
			TestGetVideoTitle("http://caress.airtv.org/audio/car.ess - 2 UFO'r'IA.mp3?miku=39", "car.ess - 2 UFO'r'IA.mp3");

		}

		[TestMethod]
		public void IsValidFor_InvalidType() {
			
			TestIsValidFor("http://vocadb.net/miku.gif", false);

		}

		[TestMethod]
		public void IsValidFor_Simple() {
			
			TestIsValidFor("http://vocadb.net/miku.mp3", true);

		}

		[TestMethod]
		public void IsValidFor_NoScheme() {
			
			TestIsValidFor("vocadb.net/miku.mp3", true);

		}

	}

}
