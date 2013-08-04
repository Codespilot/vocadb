using System.Text;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.VideoServices {

	/// <summary>
	/// Tests for <see cref="VideoServicePiapro"/>.
	/// </summary>
	[TestClass]
	public class VideoServicePiaproTests {

		private HtmlDocument SongDocument {
			get {
				return ResourceHelper.ReadHtmlDocument("piapro.htm", Encoding.UTF8);
			}
		}

		private VideoUrlParseResult ParseDocument() {
			return new VideoServicePiapro(PVService.Piapro, null, null).ParseDocument(SongDocument, "http://");
		}

		[TestMethod]
		public void Id() {
			Assert.AreEqual("61zc7sceslg04gcx", ParseDocument().Id, "Id");
		}

		[TestMethod]
		public void Title() {
			Assert.AreEqual("マトリョシカ　オケ", ParseDocument().Title, "Title");	
		}

		[TestMethod]
		public void Length() {
			Assert.AreEqual(201, ParseDocument().LengthSeconds, "Length");
		}

		[TestMethod]
		public void Author() {
			Assert.AreEqual("ハチ", ParseDocument().Author, "Author");
		}

	}

}
