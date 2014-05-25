using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	[TestClass]
	public class EntryQueriesTests {

		private FakeAlbumRepository repository;
		private EntryQueries queries;

		private EntryForApiContract AssertHasEntry(PartialFindResult<EntryForApiContract> result, string name, EntryType entryType) {
			
			var entry = result.Items.FirstOrDefault(a => string.Equals(a.DefaultName, name, StringComparison.InvariantCultureIgnoreCase)
				&& a.EntryType == entryType);

			Assert.IsNotNull(entry, "Entry found");
			return entry;

		}

		private PartialFindResult<EntryForApiContract> CallGetList(string query, 
			string tag = null,
			EntryStatus? status = null,
			int start = 0, int maxResults = 10, bool getTotalCount = true,
			NameMatchMode nameMatchMode = NameMatchMode.Words,
			EntryOptionalFields fields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			bool ssl = false) {
			
			return queries.GetList(query, tag, status, start, maxResults, getTotalCount, nameMatchMode, fields, lang, ssl);

		}

		[TestInitialize]
		public void SetUp() {
			
			repository = new FakeAlbumRepository();
			var permissionContext = new FakePermissionContext();
			var thumbPersister = new InMemoryImagePersister();

			queries = new EntryQueries(repository, permissionContext, thumbPersister);

			var group = CreateEntry.Artist(ArtistType.OtherGroup, name: "1640mP");
			var artist = CreateEntry.Producer(name: "40mP");
			var artist2 = CreateEntry.Producer(name: "Tripshots");
			var album = CreateEntry.Album(name: "40mP Piano Arrange Album");
			var song = CreateEntry.Song(name: "Mosaik Role [40mP ver.]");

			repository.Save(group, artist, artist2);
			repository.Save(album);
			repository.Save(song);

		}

		[TestMethod]
		public void List() {
			
			var result = CallGetList(query: "40mP");

			Assert.AreEqual(4, result.TotalCount, "TotalCount");
			Assert.AreEqual(4, result.Items.Length, "Items.Length");

			AssertHasEntry(result, "40mP", EntryType.Artist);
			AssertHasEntry(result, "1640mP", EntryType.Artist);
			AssertHasEntry(result, "40mP Piano Arrange Album", EntryType.Album);
			AssertHasEntry(result, "Mosaik Role [40mP ver.]", EntryType.Song);

		}

		[TestMethod]
		public void List_CanonizedArtistName() {
			
			var artist = CreateEntry.Producer(name: "nightmare-P");
			repository.Save(artist);

			var resultExact = CallGetList(query: "nightmare-P");
			var resultVariant = CallGetList(query: "nightmareP");
			var resultPartial = CallGetList(query: "nightmare");

			AssertHasEntry(resultExact, "nightmare-P", EntryType.Artist);
			AssertHasEntry(resultVariant, "nightmare-P", EntryType.Artist);
			AssertHasEntry(resultPartial, "nightmare-P", EntryType.Artist);

		}

	}

}
