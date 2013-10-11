using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Helpers {

	/// <summary>
	/// Tests for <see cref="FollowedArtistNotifier"/>
	/// </summary>
	[TestClass]
	public class FollowedArtistNotifierTests {

		private Album album;
		private User creator;
		private FakeEntryLinkFactory entryLinkFactory;
		private Artist producer;
		private FakeRepository<UserMessage> repository;  
		private User user;
		private Artist vocalist;

		private void CallSendNotifications(IUser creator) {

			repository.HandleTransaction(ctx => {
				new FollowedArtistNotifier().SendNotifications(ctx, album, new[] { producer, vocalist }, creator, entryLinkFactory);
			});

		}

		[TestInitialize]
		public void SetUp() {

			entryLinkFactory = new FakeEntryLinkFactory();
			album = new Album(new LocalizedString("New Album", ContentLanguageSelection.English));
			producer = new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer };
			vocalist = new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 2, ArtistType = ArtistType.Vocaloid };
			user = new User("Miku", "123", string.Empty, 0) { Id = 1};
			user.AddArtist(producer);
			creator = new User("Rin", "123", string.Empty, 0) { Id = 2 };
			repository = new FakeRepository<UserMessage>();

		}

		[TestMethod]
		public void SendNotifications() {
			

			CallSendNotifications(creator);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user, notification.Receiver, "Receiver");
			Assert.AreEqual("New album by Tripshots", notification.Subject, "Subject");

		}

		[TestMethod]
		public void SendNotifications_SameUser() {

			CallSendNotifications(user);

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification created");

		}

		[TestMethod]
		public void SendNotifications_MultipleFollowedArtists() {

			user.AddArtist(vocalist);

			CallSendNotifications(creator);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual("New album", notification.Subject, "Subject");

		}

		[TestMethod]	
		public void TooManyUnreadMessages() {

			for (int i = 0; i < 5; ++i)
				repository.Add(new UserMessage(user, "New message!", i.ToString(), false));

			CallSendNotifications(creator);

			Assert.AreEqual(5, repository.List<UserMessage>().Count, "No notification created");
			Assert.IsTrue(repository.List<UserMessage>().All(m => m.Subject == "New message!"), "No notification created");

		}

	}

}
