using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Users {

	[TestClass]
	public class UserTests {

		private User user;

		[TestInitialize]
		public void SetUp() {

			user = new User();

		}

		[TestMethod]
		public void AddOwnedArtist_New() {

			var artist = new Artist { Id = 1 };

			var result = user.AddOwnedArtist(artist);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(artist, result.Artist, "Artist");

		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddOwnedArtist_AlreadyAdded() {

			var artist = new Artist { Id = 1 };

			user.AddOwnedArtist(artist);
			user.AddOwnedArtist(artist);

		}

		[TestMethod]
		public void CreateWebLink() {

			user.CreateWebLink(new WebLinkContract("http://www.test.com", "test link", WebLinkCategory.Other));

			Assert.AreEqual(1, user.WebLinks.Count, "Should have one link");
			var link = user.WebLinks.First();
			Assert.AreEqual("test link", link.Description, "description");
			Assert.AreEqual("http://www.test.com", link.Url, "url");
			Assert.AreEqual(WebLinkCategory.Other, link.Category, "category");

		}

	}

}
