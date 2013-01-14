using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	[TestClass]
	public class ArtistHelperTests {

		private IArtistWithSupport animator;
		private IArtistWithSupport producer;
		private IArtistWithSupport producer2;
		private IArtistWithSupport vocalist;

		private IArtistWithSupport CreateArtist(ArtistType artistType, string name) {

			var p = new Artist { ArtistType = artistType };
			p.Names.Add(new ArtistName(p, new LocalizedString(name, ContentLanguageSelection.English)));
			return p.AddAlbum(new Album());

		}

		private TranslatedStringWithDefault CreateString(TranslatedString translatedString) {
			return new TranslatedStringWithDefault(translatedString.Japanese, translatedString.Romaji, translatedString.English, translatedString.Default ?? string.Empty);
		}

		[TestInitialize]
		public void SetUp() {

			animator = CreateArtist(ArtistType.Animator, "wakamuraP");
			producer = CreateArtist(ArtistType.Producer, "devilishP");
			producer2 = CreateArtist(ArtistType.Producer, "40mP");
			vocalist = CreateArtist(ArtistType.Vocaloid, "Hatsune Miku");

		}

		[TestMethod]
		public void GetCanonizedName_NotPName() {

			var result = ArtistHelper.GetCanonizedName("devilish5150");

			Assert.AreEqual("devilish5150", result, "result");

		}

		[TestMethod]
		public void GetCanonizedName_PName() {

			var result = ArtistHelper.GetCanonizedName("devilishP");

			Assert.AreEqual("devilish", result, "result");

		}

		[TestMethod]
		public void GetCanonizedName_PDashName() {

			var result = ArtistHelper.GetCanonizedName("devilish-P");

			Assert.AreEqual("devilish", result, "result");

		}

		[TestMethod]
		public void GetArtistString_Empty() {

			var result = ArtistHelper.GetArtistString(new IArtistWithSupport[] { }, false);

			Assert.AreEqual(CreateString(TranslatedString.Create(string.Empty)), result, "result is empty");

		}

		[TestMethod]
		public void GetArtistString_OneProducer() {

			var result = ArtistHelper.GetArtistString(new[] { producer }, false);

			Assert.AreEqual(CreateString(producer.Artist.Names.SortNames), result, "producer's name");

		}

		[TestMethod]
		public void GetArtistString_TwoProducers() {

			var result = ArtistHelper.GetArtistString(new[] { producer, producer2 }, false);

			Assert.AreEqual(CreateString(TranslatedString.Create(producer.Artist.DefaultName + ", " + producer2.Artist.DefaultName)), result, "artist string has both producers");

		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVocalist() {

			var result = ArtistHelper.GetArtistString(new[] { producer, vocalist }, false);

			Assert.AreEqual(CreateString(TranslatedString.Create(producer.Artist.DefaultName + " feat. " + vocalist.Artist.DefaultName)), result, "artist string has producer and vocalist name");

		}

		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_NotVideo() {

			var result = ArtistHelper.GetArtistString(new[] { producer, animator }, false);

			Assert.AreEqual(CreateString(producer.Artist.Names.SortNames), result, "artist string has one producer");

		}

		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_IsVideo() {

			var result = ArtistHelper.GetArtistString(new[] { producer, animator }, true);

			Assert.AreEqual(CreateString(TranslatedString.Create(producer.Artist.DefaultName + ", " + animator.Artist.DefaultName)), result, "artist string has one producer and animator");

		}

	}

}
