﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Code;

namespace VocaDb.Tests.Web.Code {

	/// <summary>
	/// Tests for <see cref="EntryAnchorFactory"/>.
	/// </summary>
	[TestClass]
	public class EntryAnchorFactoryTests {

		private EntryAnchorFactory factory;

		[TestInitialize]
		public void SetUp() {
			factory = new EntryAnchorFactory();
		}

		[TestMethod]
		public void CreateEntryLink_Components() {

			var result = factory.CreateEntryLink(EntryType.Artist, 39, "Hatsune Miku");

			Assert.AreEqual("<a href=\"/Artist/Details/39\">Hatsune Miku</a>", result, "result");

		}

		[TestMethod]
		public void CreateEntryLink_Entry() {

			var artist = new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 39 };
			var result = factory.CreateEntryLink(artist);

			Assert.AreEqual("<a href=\"/Artist/Details/39\">Hatsune Miku</a>", result, "result");

		}

	}
}
