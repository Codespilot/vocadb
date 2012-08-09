using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public string[] FindNames(string query) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] {};

			query = query.Trim();
			const int maxResults = 10;

			return HandleQuery(session => {

				var artistNames = session.Query<ArtistName>()
					.AddEntryNameFilter(query, NameMatchMode.Auto)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumNames = session.Query<AlbumName>()
					.AddEntryNameFilter(query, NameMatchMode.Auto)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songNames = session.Query<SongName>()
					.AddEntryNameFilter(query, NameMatchMode.Auto)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var tagNames =
					session.Query<Tag>()
					.Where(t => t.Name.Contains(query))
					.OrderBy(t => t.Name)
					.Select(t => t.Name)
					.Take(maxResults)
					.ToArray();

				var allNames = artistNames
					.Concat(albumNames)
					.Concat(songNames)
					.Concat(tagNames)
					.Distinct()
					.OrderBy(n => n)
					.Take(maxResults);

				return allNames.ToArray();

			});

		}

		public AllEntriesSearchResult Find(string query, int maxResults, bool getTotalCount) {

			if (string.IsNullOrWhiteSpace(query))
				return new AllEntriesSearchResult();

			var canonized = ArtistHelper.GetCanonizedName(query);
			var matchMode = FindHelpers.GetMatchMode(query, NameMatchMode.Auto);

			return HandleQuery(session => {

				var artists = 
					session.Query<ArtistName>()
					.AddArtistNameFilter(query, canonized, matchMode)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var artistCount = (getTotalCount ?
					session.Query<ArtistName>()
					.AddArtistNameFilter(query, canonized, matchMode)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.Distinct()
					.Count() 
					: 0);

				var albums = 
					session.Query<AlbumName>()
					.AddEntryNameFilter(query, matchMode)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumCount = (getTotalCount ?
					session.Query<AlbumName>()
					.AddEntryNameFilter(query, matchMode)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.Distinct()
					.Count()
					: 0);

				var songs = 
					session.Query<SongName>()
					.AddEntryNameFilter(query, matchMode)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songCount = (getTotalCount ?
					session.Query<SongName>()
					.AddEntryNameFilter(query, matchMode)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.Distinct()
					.Count()
					: 0);

				var tags =
					session.Query<Tag>()
					.Where(t => t.Name.Contains(query))
					.OrderBy(t => t.Name)
					.Take(maxResults)
					.ToArray();

				var tagCount = (getTotalCount ?
					session.Query<Tag>()
					.Where(t => t.Name.Contains(query))
					.Distinct()
					.Count()
					: 0);

				var artistResult = new PartialFindResult<ArtistWithAdditionalNamesContract>(
					artists.Select(a => new ArtistWithAdditionalNamesContract(a, PermissionContext.LanguagePreference)).ToArray(), artistCount);

				var albumResult = new PartialFindResult<AlbumWithAdditionalNamesContract>(
					albums.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference)).ToArray(), albumCount);

				var songResult = new PartialFindResult<SongWithAlbumContract>(
					songs.Select(a => new SongWithAlbumContract(a, PermissionContext.LanguagePreference)).ToArray(), songCount);

				var tagResult = new PartialFindResult<TagContract>(
					tags.Select(a => new TagContract(a)).ToArray(), tagCount);

				return new AllEntriesSearchResult(query, albumResult, artistResult, songResult, tagResult);

			});

		}

		public PartialFindResult<ActivityEntryContract> GetActivityEntries(int maxEntries) {

			return GetActivityEntries(0, maxEntries);

		}

		public PartialFindResult<ActivityEntryContract> GetActivityEntries(int start, int maxEntries) {

			return HandleQuery(session => {

				var entries = session.Query<ActivityEntry>().OrderByDescending(a => a.CreateDate).Skip(start).Take(maxEntries).ToArray();

				var contracts = entries
					.Where(e => !e.EntryBase.Deleted)
					.Select(e => new ActivityEntryContract(e, PermissionContext.LanguagePreference))
					.ToArray();

				var count = session.Query<ActivityEntry>().Count();

				return new PartialFindResult<ActivityEntryContract>(contracts, count);

			});

		}

		public FrontPageContract GetFrontPageContent() {

			const int maxNewsEntries = 4;
			const int maxActivityEntries = 20;

			return HandleQuery(session => {

				var activityEntries = session.Query<ActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Take(maxActivityEntries)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted);

				var newsEntries = session.Query<NewsEntry>().Where(n => n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries).ToArray();

				if (newsEntries.Length < maxNewsEntries)
					newsEntries = newsEntries.Concat(session.Query<NewsEntry>().Where(n => !n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries - newsEntries.Length)).ToArray();

				var topAlbums = session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.RatingAverageInt)
					.OrderByDescending(a => a.RatingCount)
					.Take(10).ToArray();
				var newAlbums = session.Query<Album>().Where(a => !a.Deleted 
					&& a.OriginalRelease.ReleaseDate.Year != null 
					&& a.OriginalRelease.ReleaseDate.Month != null 
					&& a.OriginalRelease.ReleaseDate.Day != null)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Month)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Day)
					.Take(10).ToArray();

				var newSongs = session.Query<Song>().Where(s => !s.Deleted && s.PVServices != PVServices.Nothing)
					.OrderByDescending(s => s.CreateDate)
					.Take(20).ToArray();

				return new FrontPageContract(activityEntries, newsEntries, newAlbums, topAlbums, newSongs, PermissionContext.LanguagePreference);

			});

		}

	}
}
