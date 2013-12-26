using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;
using VocaDb.Tests.TestData;
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
		private UserMessage receivedMessage;
		private User receiver;
		private UserMessage sentMessage;

		private UserMessageContract CallGet(int id) {
			return queries.Get(id, null);
		}

		[TestInitialize]
		public void SetUp() {
			
			sender = new User { Name = "Sender user", Id = 1};
			receiver = new User { Name = "Receiver user", Id = 2 };
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(receiver, ContentLanguagePreference.Default));

			receivedMessage = CreateEntry.UserMessage(1, "Hello world", "Message body", sender, receiver);
			sentMessage = CreateEntry.UserMessage(2, "Hello to you too", "Message body", receiver, sender);

			repository = new FakeUserMessageRepository(sentMessage, receivedMessage);

			queries = new UserMessageQueries(repository, permissionContext);

		}

		[TestMethod]
		public void Get() {

			var result = CallGet(1);

			Assert.IsNotNull(result, "Message was loaded");
			Assert.AreEqual("Hello world", result.Subject, "Message subject");
			Assert.AreEqual("Message body", result.Body, "Message body");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Get_NoPermission() {

			var msg = new UserMessage(sender, sender, "Hello world", "Message body", false) { Id = 39 };
			repository.Add(msg);

			CallGet(39);

		}

		[TestMethod]
		public void GetList() { 

			var result = queries.GetList(receiver.Id, new PagingProperties(0, 10, false), false, new FakeUserIconFactory());

			Assert.AreEqual(1, result.ReceivedMessages.Length, "Number of received messages");
			Assert.AreEqual(1, result.SentMessages.Length, "Number of sent messages");
			Assert.AreEqual("Hello world", result.ReceivedMessages.First().Subject, "Received message subject");
			Assert.AreEqual("Hello to you too", result.SentMessages.First().Subject, "Sent message subject");

		}

		// TODO: more test cases for unread and paging

	}

}
