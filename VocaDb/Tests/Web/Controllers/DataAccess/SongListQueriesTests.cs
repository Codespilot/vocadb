using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="SongListQueries"/>.
	/// </summary>
	[TestClass]
	public class SongListQueriesTests {

		private FakePermissionContext permissionContext;
		private FakeSongListRepository repository;
		private SongListForEditContract songListContract;
		private SongListQueries queries;
		private User userWithSongList;

		private SongInListEditContract[] SongInListEditContracts(params Song[] songs) {
			return songs.Select(s => new SongInListEditContract(new SongContract(s, ContentLanguagePreference.Default))).ToArray();
		}

		[TestInitialize]
		public void SetUp() {
			
			repository = new FakeSongListRepository();
			userWithSongList = new User("User with songlist", "123", "test@test.com", 123);
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(userWithSongList, ContentLanguagePreference.Default));

			queries = new SongListQueries(repository, permissionContext, new FakeEntryLinkFactory());

			var song1 = new Song(TranslatedString.Create("Project Diva desu.")) { Id = 1};
			var song2 = new Song(TranslatedString.Create("World is Mine")) { Id = 2};

			repository.Add(userWithSongList);
			repository.Add(song1, song2);

			songListContract = new SongListForEditContract {
				Name = "Mikunopolis Setlist",
				Description = "MIKUNOPOLIS in LOS ANGELES - Hatsune Miku US debut concert held at Nokia Theatre for Anime Expo 2011 on 2nd July 2011.",
				SongLinks = SongInListEditContracts(song1, song2)
			};

		}

		[TestMethod]
		public void Create() {

			queries.UpdateSongList(songListContract, null);

			var songList = repository.List<SongList>().FirstOrDefault();
			Assert.IsNotNull(songList, "List was saved to repository");

			Assert.AreEqual(songListContract.Name, songList.Name, "Name");
			Assert.AreEqual(songListContract.Description, songList.Description, "Description");
			Assert.AreEqual(2, songList.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Project Diva desu.", songList.AllSongs[0].Song.DefaultName, "First song as expected");
			Assert.AreEqual("World is Mine", songList.AllSongs[1].Song.DefaultName, "Second song as expected");

		}

		[TestMethod]
		public void UpdateSongLinks() {

			queries.UpdateSongList(songListContract, null);

			var newSong = new Song(TranslatedString.Create("Electric Angel"));
			repository.Add(newSong);

			songListContract.SongLinks = songListContract.SongLinks.Concat(SongInListEditContracts(newSong)).ToArray();

			queries.UpdateSongList(songListContract, null);

			var songList = repository.List<SongList>().First();
			Assert.AreEqual(3, songList.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Electric Angel", songList.AllSongs[2].Song.DefaultName, "New song as expected");

		}

	}

}
