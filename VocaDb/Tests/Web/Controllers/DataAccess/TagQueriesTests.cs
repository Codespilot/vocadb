using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
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

		private FakePermissionContext permissionContext;
		private TagQueries queries;
		private FakeTagRepository repository;
		private Tag tag;
		private User user;

		[TestInitialize]
		public void SetUp() {

			tag = new Tag("Appearance_Miku") {
				TagName = "Appearance_Miku",
				Description = ""
			};
			repository = new FakeTagRepository(tag);

			user = new User("User", "123", "test@test.com", 123);
			repository.Add(user);

			permissionContext = new FakePermissionContext(new UserContract(user));

			queries = new TagQueries(repository, permissionContext);


		}

		[TestMethod]
		public void Update_Description() {

			var updated = new TagContract(tag);
			updated.Description = "mikumikudance.wikia.com/wiki/Miku_Hatsune_Appearance_(Mamama)";

			queries.UpdateTag(updated, null);

			Assert.AreEqual(updated.Description, tag.Description, "Description was updated");

		}

	}
}
