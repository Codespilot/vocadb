using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="TagQueries"/>.
	/// </summary>
	[TestClass]
	public class TagQueriesTests {

		private InMemoryImagePersister imagePersister;
		private FakePermissionContext permissionContext;
		private TagQueries queries;
		private FakeTagRepository repository;
		private Tag tag;
		private User user;

		private Stream TestImage() {
			return ResourceHelper.GetFileStream("yokohma_bay_concert.jpg");
		}

		[TestInitialize]
		public void SetUp() {

			tag = new Tag("Appearance_Miku") {
				Id = 1,
				TagName = "Appearance_Miku",
				Description = ""
			};
			repository = new FakeTagRepository(tag);

			user = new User("User", "123", "test@test.com", 123);
			repository.Add(user);

			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(user, ContentLanguagePreference.Default));

			imagePersister = new InMemoryImagePersister();
			queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), imagePersister);

		}

		[TestMethod]
		public void Update_Description() {

			var updated = new TagContract(tag);
			updated.Description = "mikumikudance.wikia.com/wiki/Miku_Hatsune_Appearance_(Mamama)";

			queries.UpdateTag(updated, null);

			Assert.AreEqual(updated.Description, tag.Description, "Description was updated");

			var archivedVersion = repository.List<ArchivedTagVersion>().FirstOrDefault(a => a.Tag.Id == tag.Id);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Description, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_Image() {
			
			var updated = new TagContract(tag);
			using (var stream = TestImage()) {
				queries.UpdateTag(updated, new UploadedFileContract { Mime = MediaTypeNames.Image.Jpeg, Stream = stream });			
			}

			var thumb = new EntryThumb(tag, MediaTypeNames.Image.Jpeg);
			Assert.IsTrue(imagePersister.HasImage(thumb, ImageSize.Original), "Original image was saved");
			Assert.IsTrue(imagePersister.HasImage(thumb, ImageSize.SmallThumb), "Small thumbnail was saved");

			var archivedVersion = repository.List<ArchivedTagVersion>().FirstOrDefault(a => a.Tag.Id == tag.Id);
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(TagEditableFields.Picture, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

	}
}
