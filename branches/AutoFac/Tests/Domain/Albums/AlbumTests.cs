using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Albums {

	[TestClass]
	public class AlbumTests {

		private Album album;

		[TestInitialize]
		public void SetUp() {

			album = new Album();

		}

		[TestMethod]
		public void Ctor_LocalizedString() {

			var result = new Album(new LocalizedString("album", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, result.Names.Count(), "Names count");
			Assert.IsTrue(result.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(result.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("album", result.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");

		}

		[TestMethod]
		public void CreateWebLink() {

			album.CreateWebLink("test link", "http://www.test.com", WebLinkCategory.Other);

			Assert.AreEqual(1, album.WebLinks.Count, "Should have one link");
			var link = album.WebLinks.First();
			Assert.AreEqual("test link", link.Description, "description");
			Assert.AreEqual("http://www.test.com", link.Url, "url");
			Assert.AreEqual(WebLinkCategory.Other, link.Category, "category");

		}

	}

}
