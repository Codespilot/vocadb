using System;
using System.Diagnostics;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.Search.SongSearch {

	[TestClass]
	public class SongSearchDatabaseTests {

		private static IContainer container;
		private SongQueryParams queryParams;

		private static Artist artist;
		private static Song song;
		private static Song song2;
		private static Song songWithArtist;
		private static Tag tag;

		private static void HandleTransaction(Action<IRepositoryContext<Song>> act) {
			
			using (container.BeginLifetimeScope()) {
				
				var repo = container.Resolve<ISongRepository>();
				repo.HandleTransaction(act);
					
			}

		}

		[ClassInitialize]
		public static void ClassSetUp(TestContext testContext) {
			
			container = TestContainerFactory.BuildContainer();
			HandleTransaction(ctx => {
				
				artist = new Artist(TranslatedString.Create("Junk")) { Id = 257 };
				ctx.Save(artist);

				tag = new Tag("Electronic");
				ctx.Save(tag);

				song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English)) { Id = 121, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1) };
				song.Tags.Usages.Add(new SongTagUsage(song, tag));
				ctx.Save(song);

				song2 = new Song(new LocalizedString("Tears of Palm", ContentLanguageSelection.English)) {
					Id = 121, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1)
				};
				ctx.Save(song2);

				songWithArtist = new Song(new LocalizedString("Crystal Tears", ContentLanguageSelection.English)) { Id = 7787, FavoritedTimes = 39, CreateDate = new DateTime(2012, 1, 1) };
				songWithArtist.AddArtist(artist);
				ctx.Save(songWithArtist);
					
			});					

		}

		[TestInitialize]
		public void SetUp() {
			
			queryParams = new SongQueryParams { SortRule = SongSortRule.Name };

		}

		private PartialFindResult<Song> CallFind(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default) {
			
			using (container.BeginLifetimeScope()) {

				var search = new Model.Service.Search.SongSearch.SongSearch(container.Resolve<Model.Service.Search.IQuerySource>(), languagePreference);

				var watch = new Stopwatch();
				watch.Start();

				var result = search.Find(queryParams);

				Console.WriteLine("Test finished in {0}ms", watch.ElapsedMilliseconds);

				return result;

			}

		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		public void ListAll() {

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual(songWithArtist, result.Items[0]);
			Assert.AreEqual(song.DefaultName, result.Items[1].DefaultName);

		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		public void ListSkip() {

			queryParams.Paging.Start = 1;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual(song, result.Items[0]);

		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		public void ListSortName() {

			queryParams.SortRule = SongSortRule.Name;

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortFavorites() {

			queryParams.SortRule = SongSortRule.FavoritedTimes;

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortAdditionDate() {

			queryParams.SortRule = SongSortRule.AdditionDate;

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual("Nebula", result.Items[0].DefaultName);
			Assert.AreEqual("Tears of Palm", result.Items[1].DefaultName);
			Assert.AreEqual("Crystal Tears", result.Items[2].DefaultName);

		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		public void QueryName() {

			queryParams.Common.Query = "Crystal Tears";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Query by name as words.
		/// </summary>
		[TestMethod]
		public void QueryNameWords() {

			queryParams.Common.NameMatchMode = NameMatchMode.Words;
			queryParams.Common.Query = "Tears Crystal";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Query by name, move exact result to top.
		/// 
		/// 2 songs match, Tears of Palm and Crystal Tears.
		/// Tears of Palm is later in the results when sorted by title, 
		/// but matches from the beginning so it should be moved to first.
		/// </summary>
		[TestMethod]
		public void QueryNameMoveExactToTop() {

			queryParams.Common.Query = "Tears";
			queryParams.Common.MoveExactToTop = true;
			queryParams.Paging.MaxEntries = 1;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(2, result.TotalCount, "2 total count");
			Assert.AreEqual(song2, result.Items[0], "result is as expected");

		}

		/// <summary>
		/// Query by tag
		/// </summary>
		[TestMethod]
		public void QueryTag() {

			queryParams.Common.Query = "tag:Electronic";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "1 total count");
			Assert.AreEqual(song, result.Items[0], "result is as expected");

		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		public void QueryType() {

			queryParams.SongTypes = new[] { SongType.Original };

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			Assert.AreEqual(song, result.Items[0]);

		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		public void QueryArtist() {

			queryParams.ArtistId = artist.Id;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "Number of results");
			Assert.AreEqual(1, result.TotalCount, "Total result count");
			Assert.AreEqual(songWithArtist, result.Items[0], "songs are equal");

		}

		/// <summary>
		/// Query songs with only PVs.
		/// </summary>
		[TestMethod]
		public void QueryOnlyWithPVs() {

			queryParams.OnlyWithPVs = true;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			Assert.AreEqual(song, result.Items[0], "songs are equal");

		}

	}

}
