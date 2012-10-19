using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.Domain.Songs {

	/// <summary>
	/// Tests for <see cref="Song"/>.
	/// </summary>
	[TestClass]
	public class SongTests {

		private Song song;

		private void CreatePV(PVService service) {
			song.CreatePV(new PVContract { Service = service, PVId = "test", Name = "test" });
		}

		[TestInitialize]
		public void Setup() {

			song = new Song();

		}

		[TestMethod]
		public void Ctor_LocalizedString() {

			song = new Song(new LocalizedString("song", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, song.Names.Count(), "Names count");
			Assert.IsTrue(song.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(song.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("song", song.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");

		}

		[TestMethod]
		public void UpdatePVServices_None() {

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.Nothing, song.PVServices);

		}

		[TestMethod]
		public void UpdatePVServices_One() {

			CreatePV(PVService.Youtube);

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.Youtube, song.PVServices);

		}

		[TestMethod]
		public void UpdatePVServices_Multiple() {

			CreatePV(PVService.NicoNicoDouga);
			CreatePV(PVService.SoundCloud);
			CreatePV(PVService.Youtube);

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.NicoNicoDouga | PVServices.SoundCloud | PVServices.Youtube, song.PVServices);

		}

	}

}
