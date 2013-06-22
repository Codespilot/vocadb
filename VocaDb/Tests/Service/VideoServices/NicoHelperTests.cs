using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices {

	/// <summary>
	/// Unit tests for <see cref="NicoHelper"/>.
	/// </summary>
	[TestClass]
	public class NicoHelperTests {

		private Artist ArtistFunc(string name) {

			var artistNames = new HashSet<string> { "重音テト", "鏡音リン", "初音ミク", "MEIKO" };

			if (!artistNames.Contains(name))
				return null;

			return new Artist(TranslatedString.Create(name));

		}

		[TestInitialize]
		public void SetUp() {
			
		}

		[TestMethod]
		public void ParseLength_LessThan10Mins() {

			var result = NicoHelper.ParseLength("3:09");

			Assert.AreEqual(189, result, "result");

		}

		[TestMethod]
		public void ParseLength_MoreThan10Mins() {

			var result = NicoHelper.ParseLength("39:39");

			Assert.AreEqual(2379, result, "result");

		}

		[TestMethod]
		public void ParseLength_MoreThan60Mins() {

			var result = NicoHelper.ParseLength("339:39");

			Assert.AreEqual(20379, result, "result");

		}

		/// <summary>
		/// Valid title, basic test case
		/// </summary>
		[TestMethod]
		public void ParseTitle_Valid() {

			var result = NicoHelper.ParseTitle("【重音テト】 ハイゲインワンダーランド 【オリジナル】", ArtistFunc);

			Assert.AreEqual(1, result.Artists.Count, "1 artist");
			Assert.AreEqual("重音テト", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("ハイゲインワンダーランド", result.Title, "title");
			Assert.AreEqual(SongType.Original, result.SongType, "song type");

		}

		/// <summary>
		/// Skip whitespace in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_WhiteSpace() {

			var result = NicoHelper.ParseTitle("【 鏡音リン 】　sister's noise　(FULL) 【とある科学の超電磁砲S】", ArtistFunc);

			Assert.AreEqual(1, result.Artists.Count, "1 artist");
			Assert.AreEqual("鏡音リン", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("sister's noise　(FULL)", result.Title, "title");

		}

		/// <summary>
		/// Handle special characters in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_SpecialChars() {

			var result = NicoHelper.ParseTitle("【巡音ルカ･Lily】Blame of Angel", ArtistFunc);

			// TODO: might be able to handle artists as well.
			Assert.AreEqual("Blame of Angel", result.Title, "title");

		}

		/// <summary>
		/// Handle special characters in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_MultipleArtistFields() {

			var result = NicoHelper.ParseTitle("【初音ミク】恋風～liebe wind～【鏡音リン】", ArtistFunc);

			Assert.AreEqual(2, result.Artists.Count, "2 artists");
			Assert.AreEqual("初音ミク", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("鏡音リン", result.Artists[1].DefaultName, "artist");
			Assert.AreEqual("恋風～liebe wind～", result.Title, "title");

		}

		/// <summary>
		/// Handle artist with original.
		/// </summary>
		[TestMethod]
		public void ParseTitle_ArtistOriginal() {

			var result = NicoHelper.ParseTitle("libido / L.A.M.B 【MEIKOオリジナル】", ArtistFunc);

			Assert.AreEqual(1, result.Artists.Count, "1 artist");
			Assert.AreEqual("MEIKO", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("libido / L.A.M.B", result.Title, "title");
			Assert.AreEqual(SongType.Original, result.SongType, "song type");

		}
	}

}
