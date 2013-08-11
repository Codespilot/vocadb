using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
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
		private FakePermissionContext permissionContext;
		private FakeUserRepository repository;
		private User userWithEmail;
		private User userWithoutEmail;

		private User GetUserFromRepo(string username) {
			return repository.List<User>().FirstOrDefault(u => u.Name == username);
		}

		[TestInitialize]
		public void SetUp() {

			userWithEmail = new User("already_exists", "123", "already_in_use@vocadb.net", 123) { Id = 123 };
			userWithoutEmail = new User("no_email", "222", string.Empty, 321) { Id = 321 };
			repository = new FakeUserRepository(userWithEmail, userWithoutEmail);
			permissionContext = new FakePermissionContext(new UserContract(userWithEmail));
			data = new UserQueries(repository, permissionContext, new FakeEntryLinkFactory());

		}

		[TestMethod]
		public void Create() {

			var name = "hatsune_miku";
			var result = data.Create(name, "3939", "mikumiku@crypton.jp", "crypton.jp", TimeSpan.FromMinutes(39));

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual(name, result.Name, "Name");

			var user = GetUserFromRepo(name);
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

		[TestMethod]
		public void CreateTwitter() {

			var name = "hatsune_miku";
			var result = data.CreateTwitter("auth_token", name, "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp");

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual(name, result.Name, "Name");

			var user = GetUserFromRepo(name);
			Assert.IsNotNull(user, "User found in repository");
			Assert.AreEqual(name, user.Name, "Name");
			Assert.AreEqual("mikumiku@crypton.jp", user.Email, "Email");
			Assert.AreEqual(UserGroupId.Regular, user.GroupId, "GroupId");

			Assert.AreEqual("auth_token", user.Options.TwitterOAuthToken, "TwitterOAuthToken");
			Assert.AreEqual(39, user.Options.TwitterId, "TwitterId");
			Assert.AreEqual("Miku_Crypton", user.Options.TwitterName, "TwitterName");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void CreateTwitter_NameAlreadyExists() {

			data.CreateTwitter("auth_token", "already_exists", "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp");

		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void CreateTwitter_EmailAlreadyExists() {

			data.CreateTwitter("auth_token", "hatsune_miku", "already_in_use@vocadb.net", 39, "Miku_Crypton", "crypton.jp");

		}

		[TestMethod]
		public void UpdateUserSettings_SetEmail() {

			var contract = new UpdateUserSettingsContract(userWithEmail) { Email = "new_email@vocadb.net" };
			var result = data.UpdateUserSettings(contract);

			Assert.IsNotNull(result, "Result");
			var user = GetUserFromRepo(userWithEmail.Name);
			Assert.IsNotNull(user, "User was found in repository");
			Assert.AreEqual("new_email@vocadb.net", user.Email, "Email");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateUserSettings_NoPermission() {

			data.UpdateUserSettings(new UpdateUserSettingsContract(userWithoutEmail));

		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void UpdateUserSettings_EmailTaken() {

			permissionContext.LoggedUser = new UserContract(userWithoutEmail);
			var contract = new UpdateUserSettingsContract(userWithoutEmail) { Email = userWithEmail.Email };

			data.UpdateUserSettings(contract);

		}

	}

}
