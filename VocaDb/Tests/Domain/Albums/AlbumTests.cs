using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Albums {

	[TestClass]
	public class AlbumTests {

		[TestMethod]
		public void Ctor_LocalizedString() {

			var result = new Album(new LocalizedString("album", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, result.Names.Count(), "Names count");
			Assert.IsTrue(result.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(result.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("album", result.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");

		}

	}

}
