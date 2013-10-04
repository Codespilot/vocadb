using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
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
		private FakePermissionContext permissionContext;
		private FakeAlbumRepository repository;
		private AlbumQueries queries;
		private User user;

		[TestInitialize]
		public void SetUp() {
			
			album = new Album(TranslatedString.Create("Synthesis")) { Id = 39 };
			repository = new FakeAlbumRepository(album);
			user = new User { Name = "Miku", GroupId = UserGroupId.Regular, Id = 1 };
			repository.Add(user);

			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(user, ContentLanguagePreference.Default));
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			queries = new AlbumQueries(repository, permissionContext, entryLinkFactory);

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
