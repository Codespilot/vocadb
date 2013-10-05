using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="AlbumQueries"/>.
	/// </summary>
	[TestClass]
	public class AlbumQueriesTests {

		private Album album;
		private CreateAlbumContract newAlbumContract;
		private FakePermissionContext permissionContext;
		private Artist producer;
		private FakeAlbumRepository repository;
		private AlbumQueries queries;
		private User user;
		private Artist vocalist;

		[TestInitialize]
		public void SetUp() {
			
			producer = new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer };
			vocalist = new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 39, ArtistType = ArtistType.Vocaloid };

			album = new Album(TranslatedString.Create("Synthesis")) { Id = 39 };
			repository = new FakeAlbumRepository(album);
			user = new User { Name = "Miku", GroupId = UserGroupId.Regular, Id = 1 };
			repository.Add(user);
			repository.Add(producer, vocalist);

			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(user, ContentLanguagePreference.Default));
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			newAlbumContract = new CreateAlbumContract {
				DiscType = DiscType.Album,
				Names = new[] {
					new LocalizedStringContract("Another Dimensions", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistContract(producer, ContentLanguagePreference.Default),
					new ArtistContract(vocalist, ContentLanguagePreference.Default), 
				}
			};

			queries = new AlbumQueries(repository, permissionContext, entryLinkFactory);

		}

		[TestMethod]
		public void Create() {

			var result = queries.Create(newAlbumContract);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Another Dimensions", result.Name, "Name");

			var album = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Another Dimensions"));

			Assert.IsNotNull(album, "Album was saved to repository");
			Assert.AreEqual("Another Dimensions", album.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, album.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, album.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(album.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", album.ArtistString.Default, "ArtistString");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			queries.Create(newAlbumContract);

		}

		[TestMethod]
		public void CreateComment() {

			var result = queries.CreateComment(39, "Hello world");
			Assert.IsNotNull(result, "Result");

			var comment = repository.List<AlbumComment>().FirstOrDefault();
			Assert.IsNotNull(comment, "Comment was saved");
			Assert.AreEqual(user, comment.Author, "Author");
			Assert.AreEqual(album, comment.Album, "Album");
			Assert.AreEqual("Hello world", comment.Message, "Comment message");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void CreateComment_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);
			
			queries.CreateComment(39, "Hello world");

		}

	}

}
