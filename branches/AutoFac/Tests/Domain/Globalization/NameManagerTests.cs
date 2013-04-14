using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Globalization {

	/// <summary>
	/// Tests for <see cref="NameManager{T}"/>.
	/// </summary>
	[TestClass]
	public class NameManagerTests {

		private NameManager<LocalizedStringWithId> nameManager;

		[TestInitialize]
		public void SetUp() {

			nameManager = new NameManager<LocalizedStringWithId>();

		}

		[TestMethod]
		public void UpdateSortNames_NoNames() {

			nameManager.UpdateSortNames();

			Assert.AreEqual(string.Empty, nameManager.SortNames[ContentLanguagePreference.Japanese], "Japanese name is empty");

		}

		[TestMethod]
		public void UpdateSortNames_OnlyPrimaryName() {

			nameManager.Add(new LocalizedStringWithId("VocaliodP", ContentLanguageSelection.English));
			nameManager.Add(new LocalizedStringWithId("ぼーかりおどP", ContentLanguageSelection.Japanese));

			nameManager.UpdateSortNames();

			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.English], "Primary English name");
			Assert.AreEqual("ぼーかりおどP", nameManager.SortNames[ContentLanguageSelection.Japanese], "Primary Japanese name");
			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.Romaji], "Primary Romaji name");

		}

		[TestMethod]
		public void UpdateSortNames_DuplicateAliases() {

			nameManager.Add(new LocalizedStringWithId("VocaliodP", ContentLanguageSelection.English));
			nameManager.Add(new LocalizedStringWithId("noa", ContentLanguageSelection.Unspecified));
			nameManager.Add(new LocalizedStringWithId("noa", ContentLanguageSelection.Unspecified));

			nameManager.UpdateSortNames();

			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.English], "Primary English name");
			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.Japanese], "Primary Japanese name");
			Assert.AreEqual("noa", nameManager.AdditionalNamesString, "Additional names");

		}

	}
}
