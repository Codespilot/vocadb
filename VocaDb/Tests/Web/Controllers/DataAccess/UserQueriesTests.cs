using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="UserQueries"/>.
	/// </summary>
	[TestClass]
	public class UserQueriesTests {

		private UserQueries data;
		private FakeUserRepository repository;

		[TestInitialize]
		public void SetUp() {

			repository = new FakeUserRepository();
			repository.Add(new User("already_exists", "123", "already_in_use@vocadb.net", 123));
			data = new UserQueries(repository);

		}

		[TestMethod]
		public void Create() {

			var name = "hatsune_miku";
			var result = data.Create(name, "3939", "mikumiku@crypton.jp", "crypton.jp", TimeSpan.FromMinutes(39));

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual(name, result.Name, "Name");

			var user = repository.List<User>().FirstOrDefault(u => u.Name == name);
			Assert.IsNotNull(user, "User found in repository");
			Assert.AreEqual(name, user.Name, "Name");
			Assert.AreEqual("mikumiku@crypton.jp", user.Email, "Email");
			Assert.AreEqual(UserGroupId.Regular, user.GroupId, "GroupId");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void Create_NameAlreadyExists() {

			data.Create("already_exists", "3939", "mikumiku@crypton.jp", "crypton.jp", TimeSpan.FromMinutes(39));

		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void Create_EmailAlreadyExists() {

			data.Create("hatsune_miku", "3939", "already_in_use@vocadb.net", "crypton.jp", TimeSpan.FromMinutes(39));

		}

	}

}
