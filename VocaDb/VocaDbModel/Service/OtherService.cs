﻿using System;
using System.Linq;
using System.Runtime.Caching;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		private AlbumContract[] GetRecentAlbums(ISession session) {

			var cacheKey = "OtherService.RecentAlbums";
			var cache = MemoryCache.Default;
			var item = (TranslatedAlbumContract[])cache.Get(cacheKey);

			if (item != null)
				return item.Select(a => new AlbumContract(a, LanguagePreference)).ToArray();

			var now = DateTime.Now;

			var upcoming = session.Query<Album>().Where(a => !a.Deleted
				&& a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year > now.Year
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year && a.OriginalRelease.ReleaseDate.Month > now.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year
					&& a.OriginalRelease.ReleaseDate.Month == now.Month
					&& a.OriginalRelease.ReleaseDate.Day > now.Day)))
				.OrderBy(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenBy(a => a.OriginalRelease.ReleaseDate.Day)
				.Take(4).ToArray();

			var recent = session.Query<Album>().Where(a => !a.Deleted
				&& a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year < now.Year
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year && a.OriginalRelease.ReleaseDate.Month < now.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year
					&& a.OriginalRelease.ReleaseDate.Month == now.Month
					&& a.OriginalRelease.ReleaseDate.Day <= now.Day)))
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day)
				.Take(3).ToArray();

			var newAlbums = upcoming.Reverse().Concat(recent)
				.Select(a => new TranslatedAlbumContract(a))
				.ToArray();

			var newAlbumContracts = upcoming.Reverse().Concat(recent)
				.Select(a => new AlbumContract(a, LanguagePreference))
				.ToArray();

			cache.Add(cacheKey, newAlbums, DateTime.Now + TimeSpan.FromHours(1));

			/*var newAlbums = session.Query<Album>().Where(a => !a.Deleted
				&& a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year < albumCutoffDate.Year
				|| (a.OriginalRelease.ReleaseDate.Year == albumCutoffDate.Year && a.OriginalRelease.ReleaseDate.Month < albumCutoffDate.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == albumCutoffDate.Year
					&& a.OriginalRelease.ReleaseDate.Month == albumCutoffDate.Month
					&& a.OriginalRelease.ReleaseDate.Day < albumCutoffDate.Day)))
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day)
				.Take(7).ToArray();*/

			return newAlbumContracts;

		}

		private UnifiedCommentContract[] GetRecentComments(ISession session, int maxComments) {

			var albumComments = session.Query<AlbumComment>().Where(c => !c.Album.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
			var artistComments = session.Query<ArtistComment>().Where(c => !c.Artist.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
			var songComments = session.Query<SongComment>().Where(c => !c.Song.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();

			var combined = albumComments.Cast<Comment>().Concat(artistComments).Concat(songComments)
				.OrderByDescending(c => c.Created)
				.Take(maxComments)
				.Select(c => new UnifiedCommentContract(c, LanguagePreference));

			return combined.ToArray();

		}

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified) {

			HandleTransaction(session => AuditLog(doingWhat, session, who, category));

		}

		public string[] FindNames(string query) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] {};

			query = query.Trim();
			const int maxResults = 10;

			var canonized = ArtistHelper.GetCanonizedName(query);
			var matchMode = FindHelpers.GetMatchMode(query, NameMatchMode.Auto);
			var words = (matchMode == NameMatchMode.Words ? FindHelpers.GetQueryWords(query) : null);

			return HandleQuery(session => {

				var artistNames = session.Query<ArtistName>()
					.FilterByArtistName(query, canonized, NameMatchMode.Auto, null)	// Can't use the existing words collection here as they are noncanonized
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumNames = session.Query<AlbumName>()
					.AddEntryNameFilter(query, NameMatchMode.Auto, words)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songNames = session.Query<SongName>()
					.AddEntryNameFilter(query, NameMatchMode.Auto, words)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var tagStr = query.Replace(' ', '_');

				var tagNames =
					session.Query<Tag>()
					.Where(t => t.Name.Contains(tagStr))
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
					.Take(maxResults)
					.ToArray();

				return NameHelper.MoveExactNamesToTop(allNames, query);

			});

		}

		public AllEntriesSearchResult Find(string query, int maxResults, bool getTotalCount) {

			if (string.IsNullOrWhiteSpace(query))
				return new AllEntriesSearchResult();

			query = query.Trim();

			var canonized = ArtistHelper.GetCanonizedName(query);
			var matchMode = FindHelpers.GetMatchMode(query, NameMatchMode.Auto, NameMatchMode.StartsWith);
			var words = (matchMode == NameMatchMode.Words ? FindHelpers.GetQueryWords(query) : null);

			return HandleQuery(session => {

				var artists = 
					session.Query<ArtistName>()
					.FilterByArtistName(query, canonized, matchMode, null) // Can't use the existing words collection here as they are noncanonized
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var artistCount = (getTotalCount ?
					session.Query<ArtistName>()
					.FilterByArtistName(query, canonized, matchMode, null)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.Distinct()
					.Count() 
					: 0);

				var albums = 
					session.Query<AlbumName>()
					.AddEntryNameFilter(query, matchMode, words)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumCount = (getTotalCount ?
					session.Query<AlbumName>()
					.AddEntryNameFilter(query, matchMode, words)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.Distinct()
					.Count()
					: 0);

				var songs = 
					session.Query<SongName>()
					.AddEntryNameFilter(query, matchMode, words)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songCount = (getTotalCount ?
					session.Query<SongName>()
					.AddEntryNameFilter(query, matchMode, words)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.Distinct()
					.Count()
					: 0);

				var tagStr = query.Replace(' ', '_');

				var tags =
					session.Query<Tag>()
					.Where(t => t.Name.Contains(tagStr))
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

		public FrontPageContract GetFrontPageContent() {

			const int maxNewsEntries = 4;
			const int maxActivityEntries = 15;

			return HandleQuery(session => {

				var activityEntries = session.Query<ActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Take(maxActivityEntries)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted);

				var newsEntries = session.Query<NewsEntry>().Where(n => n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries).ToArray();

				if (newsEntries.Length < maxNewsEntries)
					newsEntries = newsEntries.Concat(session.Query<NewsEntry>()
						.Where(n => !n.Stickied)
						.OrderByDescending(a => a.CreateDate)
						.Take(maxNewsEntries - newsEntries.Length)).ToArray();

				var topAlbums = session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.RatingAverageInt)
					.ThenByDescending(a => a.RatingCount)
					.Take(7).ToArray();

				//var albumCutoffDate = DateTime.Now.AddMonths(1);

				var newAlbums = GetRecentAlbums(session);

				/*var cutoffDate = DateTime.Now - TimeSpan.FromDays(300);

				var newSongs = session.Query<Song>()
					.Where(s => !s.Deleted && s.PVServices != PVServices.Nothing && s.CreateDate >= cutoffDate)
					.OrderByDescending(s => s.RatingScore)
					.Take(16)
					.ToArray();*/

				var newSongs = session.Query<Song>()
					.Where(s => !s.Deleted && s.PVServices != PVServices.Nothing)
					.OrderByDescending(s => s.CreateDate)
					.Take(80)
					.ToArray()
					.OrderByDescending(s => s.RatingScore)
					.Take(20)
					.ToArray();

				var firstSongVote = (newSongs.Any() ? session.Query<FavoriteSongForUser>().FirstOrDefault(s => s.Song.Id == newSongs.First().Id && s.User.Id == PermissionContext.LoggedUserId) : null);

				var recentComments = GetRecentComments(session, 7);

				return new FrontPageContract(activityEntries, newsEntries, newAlbums, recentComments, topAlbums, newSongs, 
					firstSongVote != null ? firstSongVote.Rating : SongVoteRating.Nothing, PermissionContext.LanguagePreference);

			});

		}

		public IPRule[] GetIPRules() {

			return HandleQuery(session => session.Query<IPRule>().ToArray());

		}

		public UnifiedCommentContract[] GetRecentComments() {

			return HandleQuery(session => GetRecentComments(session, 50));

		}

	}
}
