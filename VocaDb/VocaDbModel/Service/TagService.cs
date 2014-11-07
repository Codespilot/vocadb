using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class TagService : ServiceBase {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private Tag GetTag(ISession session, string name) {

			try {

				var tag = session.Load<Tag>(name);

				if (name != tag.TagName)
					tag = session.Load<Tag>(tag.TagName);

				return tag;

			} catch (ObjectNotFoundException) {
				log.Warn("Tag not found: {0}", name);
				return null;
			}

		}

		private IQueryable<T> TagUsagesQuery<T>(ISession session, string tagName) where T : TagUsage {

			return session.Query<T>().Where(a => a.Tag.Name == tagName);

		}

		private int TagUsagesCount<T, TEntry>(ISession session, string[] tagNames, Expression<Func<T, TEntry>> entryFunc)
			where T : TagUsage
			where TEntry : IEntryBase {

			return session.Query<T>().Where(a => tagNames.Contains(a.Tag.Name)).Select(entryFunc).Where(e => !e.Deleted).Distinct().Count();

		}

		private IEnumerable<TEntry> TagUsagesQuery<T, TEntry>(ISession session, string[] tagNames, int count, Expression<Func<T, TEntry>> entryFunc) 
			where T : TagUsage where TEntry : IEntryBase {

			return session.Query<T>().Where(a => tagNames.Contains(a.Tag.Name)).OrderByDescending(u => u.Count).Select(entryFunc).Where(e => !e.Deleted).Take(count).ToArray().Distinct();

		}

		public TagService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public void Archive(ISession session, Tag tag, TagDiff diff, EntryEditEvent reason) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = tag.CreateArchivedVersion(diff, agentLoginData, reason);
			session.Save(archived);

		}

		public string[] FindCategories(string query) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] { };

			return HandleQuery(session => {

				string[] tags;

				if (query.Length < 3)
					tags = session.Query<Tag>().Where(t => t.CategoryName == query).Select(t => t.CategoryName).Distinct().ToArray();
				else
					tags = session.Query<Tag>().Where(t => t.CategoryName.Contains(query)).Select(t => t.CategoryName).Distinct().ToArray();

				return tags;

			});

		}

		/// <summary>
		/// Attempts to find a single tag by name. Partial match is allowed.
		/// </summary>
		/// <param name="tagName">Tag name. Cannot be null or empty.</param>
		/// <returns>First tag that matches the name. Can be null if nothing was found.</returns>
		public TagContract FindTag(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => {

				Tag[] tags;

				if (tagName.Length < 3)
					tags = session.Query<Tag>().Where(t => t.Name == tagName).Take(1).ToArray();
				else
					tags = session.Query<Tag>().Where(t => t.Name.Contains(tagName)).Take(10).ToArray();

				var match = tags.FirstOrDefault(t => t.Name.Equals(tagName, System.StringComparison.InvariantCultureIgnoreCase));

				if (match == null)
					match = tags.FirstOrDefault();
				
				if (match == null)
					return null;

				return new TagContract(match);

			});

		}

		public PartialFindResult<AlbumTagUsageContract> GetAlbums(string tagName, int start, int maxItems) {

			return HandleQuery(session => {

				var q = session.Query<AlbumTagUsage>().Where(a => !a.Album.Deleted && a.Tag.Name == tagName);

				IQueryable<AlbumTagUsage> resultQ = q.OrderByDescending(t => t.Count);
				resultQ = resultQ.Skip(start).Take(maxItems);

				var contracts = resultQ.ToArray().Select(a => new AlbumTagUsageContract(a, PermissionContext.LanguagePreference)).ToArray();
				var totalCount = q.Count();

				return new PartialFindResult<AlbumTagUsageContract>(contracts, totalCount);

			});
		}

		public PartialFindResult<ArtistTagUsageContract> GetArtists(string tagName, int start, int maxItems) {

			return HandleQuery(session => {

				var q = session.Query<ArtistTagUsage>().Where(a => !a.Artist.Deleted && a.Tag.Name == tagName);

				IQueryable<ArtistTagUsage> resultQ = q.OrderByDescending(t => t.Count);
				resultQ = resultQ.Skip(start).Take(maxItems);

				var contracts = resultQ.ToArray().Select(a =>
					new ArtistTagUsageContract(a, PermissionContext.LanguagePreference)).ToArray();
				var totalCount = q.Count();

				return new PartialFindResult<ArtistTagUsageContract>(contracts, totalCount);

			});
		}

		public PartialFindResult<SongTagUsageContract> GetSongs(string tagName, int start, int maxItems) {

			return HandleQuery(session => {

				var q = session.Query<SongTagUsage>().Where(a => !a.Song.Deleted && a.Tag.Name == tagName);

				IQueryable<SongTagUsage> resultQ = q.OrderByDescending(t => t.Count);
				resultQ = resultQ.Skip(start).Take(maxItems);

				var contracts = resultQ.ToArray().Select(a => new SongTagUsageContract(a, PermissionContext.LanguagePreference)).ToArray();
				var totalCount = q.Count();

				return new PartialFindResult<SongTagUsageContract>(contracts, totalCount);

			});
		}

		/// <summary>
		/// Attempts to get a tag by exact name.
		/// </summary>
		/// <param name="tagName">Tag name to be matched. Can be null or empty, in which case null is returned.</param>
		/// <returns>The matched tag, if any. Can be null if nothing was found.</returns>
		public TagContract GetTag(string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return null;

			return HandleQuery(session => {

				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;

				return new TagContract(tag);

			});

		}

		private class TagTopUsagesAndCount<T> {

			public T[] TopUsages { get; set; }

			public int TotalCount { get; set; }

		}

		private TagTopUsagesAndCount<TEntry> GetTopUsagesAndCount<TUsage, TEntry, TSort>(
			ISession session, string tagName, 
			Expression<Func<TUsage, bool>> whereExpression, 
			Expression<Func<TUsage, TSort>> createDateExpression,
			Expression<Func<TUsage, TEntry>> selectExpression)
			where TUsage: TagUsage {
			
			var q = TagUsagesQuery<TUsage>(session, tagName)
				.Where(whereExpression);

			var topUsages = q
				.OrderByDescending(t => t.Count)
				.ThenByDescending(createDateExpression)
				.Select(selectExpression)
				.Take(12)
				.ToArray();

			var usageCount = q.Count();

			return new TagTopUsagesAndCount<TEntry> {
				TopUsages = topUsages, TotalCount = usageCount
			};

		}

		public TagDetailsContract GetTagDetails(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => { 
				
				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;
				
				var artists = GetTopUsagesAndCount<ArtistTagUsage, Artist, int>(session, tagName, t => !t.Artist.Deleted, t => t.Artist.Id, t => t.Artist);
				var albums = GetTopUsagesAndCount<AlbumTagUsage, Album, int>(session, tagName, t => !t.Album.Deleted, t => t.Album.RatingTotal, t => t.Album);
				var songs = GetTopUsagesAndCount<SongTagUsage, Song, int>(session, tagName, t => !t.Song.Deleted, t => t.Song.RatingScore, t => t.Song);

				return new TagDetailsContract(tag, 
					artists.TopUsages, artists.TotalCount, 
					albums.TopUsages, albums.TotalCount, 
					songs.TopUsages, songs.TotalCount, 
					PermissionContext.LanguagePreference);
				
			});

		}

		public TagForEditContract GetTagForEdit(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => {

				var inUse = session.Query<ArtistTagUsage>().Any(a => a.Tag.Name == tagName && !a.Artist.Deleted) ||
					session.Query<AlbumTagUsage>().Any(a => a.Tag.Name == tagName && !a.Album.Deleted) ||
					session.Query<SongTagUsage>().Any(a => a.Tag.Name == tagName && !a.Song.Deleted);

				var contract = new TagForEditContract(session.Load<Tag>(tagName),
					session.Query<Tag>().Select(t => t.CategoryName).OrderBy(t => t).Distinct().ToArray(), !inUse);

				return contract;

			});

		}

		public string[] GetTagNames() {

			return HandleQuery(session => session.Query<Tag>().OrderBy(t => t.Name).Select(t => t.Name).ToArray());

		}

		public TagWithArchivedVersionsContract GetTagWithArchivedVersions(string tagName) {

			return HandleQuery(session => {

				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;

				return new TagWithArchivedVersionsContract(tag);

			});

		}

		public TagCategoryContract[] GetTagsByCategories() {

			return HandleQuery(session => {

				var tags = session.Query<Tag>()
					.Where(t => t.AliasedTo == null)
					.OrderBy(t => t.Name)
					.ToArray()					
					.GroupBy(t => t.CategoryName)
					.ToArray();

				var empty = tags.Where(c => c.Key == string.Empty);

				var tagsByCategories = tags
					.Except(empty).Concat(empty)
					.Select(t => new TagCategoryContract(t.Key, t)).ToArray();

				return tagsByCategories;

			});

		}

	}

}
