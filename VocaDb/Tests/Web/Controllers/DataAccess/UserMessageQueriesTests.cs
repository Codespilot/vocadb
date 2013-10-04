using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="UserMessageQueries"/>.
	/// </summary>
	[TestClass]
	public class UserMessageQueriesTests {

		private UserMessageQueries queries;
		private FakePermissionContext permissionContext;
		private FakeUserMessageRepository repository;
		private User sender;
		private User receiver;

		[TestInitialize]
		public void SetUp() {
			
			sender = new User { Name = "Sender user", Id = 1};
			receiver = new User { Name = "Receiver user", Id = 2 };
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(receiver, ContentLanguagePreference.Default));
			repository = new FakeUserMessageRepository();

			queries = new UserMessageQueries(repository, permissionContext);

		}

		[TestMethod]
		public void Get() {

			var msg = new UserMessage(sender, receiver, "Hello world", "Message body", false) { Id = 39 };
			repository.Add(msg);

			var result = queries.Get(39);

			Assert.IsNotNull(result, "Message was loaded");
			Assert.AreEqual("Hello world", result.Subject, "Message subject");
			Assert.AreEqual("Message body", result.Body, "Message body");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Get_NoPermission() {

			var msg = new UserMessage(sender, sender, "Hello world", "Message body", false) { Id = 39 };
			repository.Add(msg);

			queries.Get(39);

		}

	}

}
