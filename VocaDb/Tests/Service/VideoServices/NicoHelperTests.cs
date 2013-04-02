using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices {

	/// <summary>
	/// Unit tests for <see cref="NicoHelper"/>.
	/// </summary>
	[TestClass]
	public class NicoHelperTests {

		[TestInitialize]
		public void SetUp() {
			
		}

		[TestMethod]
		public void ParseTitle_Valid() {

			var result = NicoHelper.ParseTitle("【重音テト】 ハイゲインワンダーランド 【オリジナル】");

			Assert.AreEqual(1, result.ArtistNames.Length, "1 artist");
			Assert.AreEqual("重音テト", result.ArtistNames.First(), "artist");
			Assert.AreEqual("ハイゲインワンダーランド", result.Title, "title");
			Assert.AreEqual(SongType.Original, result.SongType, "song type");

		}

	}

}
