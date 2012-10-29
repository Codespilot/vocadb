using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
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
