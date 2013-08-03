using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Service.AlbumImport;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.AlbumImport {

	/// <summary>
	/// Tests for <see cref="KarenTAlbumImporter"/>.
	/// </summary>
	[TestClass]
	public class KarenTAlbumImporterTests {

		private ImportedAlbumDataContract importedData;
		private HtmlDocument karenTDoc;

		[TestInitialize]
		public void SetUp() {
			karenTDoc = ResourceHelper.ReadHtmlDocument("KarenT_SystemindParadox.htm");
			importedData = new KarenTAlbumImporter().GetAlbumData(karenTDoc, "http://").Data;
		}

		[TestMethod]
		public void Title() {
			Assert.AreEqual("Systemind Paradox", importedData.Title, "Title");
		}

		[TestMethod]
		public void Description() {
			Assert.AreEqual("Heavenz 1st Album", importedData.Description, "Description");
			
		}

		[TestMethod]
		public void Artists() {
			Assert.AreEqual(1, importedData.ArtistNames.Length, "1 artist");
			Assert.AreEqual("Heavenz", importedData.ArtistNames.FirstOrDefault(), "Artist name");			
		}

		[TestMethod]
		public void Vocalists() {
			Assert.AreEqual(1, importedData.VocalistNames.Length, "1 vocalist");
			Assert.AreEqual("Hatsune Miku", importedData.VocalistNames.FirstOrDefault(), "Vocalist name");			
		}

		[TestMethod]
		public void Tracks() {

			Assert.AreEqual(11, importedData.Tracks.Length, "11 tracks");

		}

	}

}
