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
using VocaDb.Model.Helpers;
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

		public void Delete(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var tag = session.Load<Tag>(name);

				tag.Delete();

				AuditLog(string.Format("deleted {0}", tag), session);

				session.Delete(tag);

			});

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

		public string[] FindTags(string query, bool allowAliases) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] { };

			query = query.Trim().Replace(' ', '_');

			return HandleQuery(session => {

				var q = session.Query<Tag>();

				if (query.Length < 3)
					q = q.Where(t => t.Name.StartsWith(query));
				else
					q = q.Where(t => t.Name.Contains(query));

				if (!allowAliases)
					q = q.Where(t => t.AliasedTo == null);

				var tags = q
					.Take(10)
					.ToArray()
					.Select(t => t.Name)
					.ToArray();

				return tags;

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

		public TagDetailsContract GetTagDetails(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => { 
				
				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;
				
				var artists = TagUsagesQuery<ArtistTagUsage>(session, tagName).Where(a => !a.Artist.Deleted).OrderByDescending(t => t.Count).Select(u => u.Artist).Take(15).ToArray();
				var artistCount = TagUsagesQuery<ArtistTagUsage>(session, tagName).Count(a => !a.Artist.Deleted);

				var albums = TagUsagesQuery<AlbumTagUsage>(session, tagName).Where(a => !a.Album.Deleted).OrderByDescending(t => t.Count).Select(u => u.Album).Take(15).ToArray();
				var albumCount = TagUsagesQuery<AlbumTagUsage>(session, tagName).Count(a => !a.Album.Deleted);

				var songs = TagUsagesQuery<SongTagUsage>(session, tagName).Where(a => !a.Song.Deleted).OrderByDescending(t => t.Count).Select(u => u.Song).Take(15).ToArray();
				var songCount = TagUsagesQuery<SongTagUsage>(session, tagName).Count(a => !a.Song.Deleted);

				/*
				var tagNames = new[] {tag.Name}.Concat(tag.Aliases.Select(t => t.Name)).ToArray();
				
				var artists = TagUsagesQuery<ArtistTagUsage, Artist>(session, tagNames, 15, u => u.Artist);
				var artistCount = TagUsagesCount<ArtistTagUsage, Artist>(session, tagNames, u => u.Artist);

				var albums = TagUsagesQuery<AlbumTagUsage, Album>(session, tagNames, 15, u => u.Album);
				var albumCount = TagUsagesCount<AlbumTagUsage, Album>(session, tagNames, u => u.Album);

				var songs = TagUsagesQuery<SongTagUsage, Song>(session, tagNames, 15, u => u.Song);
				var songCount = TagUsagesCount<SongTagUsage, Song>(session, tagNames, u => u.Song);
				*/

				return new TagDetailsContract(tag, artists, artistCount, albums, albumCount, songs, songCount, 
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

		public void UpdateTag(TagContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			HandleTransaction(session => {

				var tag = session.Load<Tag>(contract.Name);
				var diff = new TagDiff();
				
				var oldAliasedTo = tag.AliasedTo != null ? tag.AliasedTo.Name : string.Empty;
				var newAliasedTo = contract.AliasedTo ?? string.Empty;
				if (oldAliasedTo != newAliasedTo) {
					diff.AliasedTo = true;
					tag.AliasedTo = session.Query<Tag>().FirstOrDefault(t => t.AliasedTo == null && t.Name == newAliasedTo);
				}

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName = true;

				if (tag.Description != contract.Description)
					diff.Description = true;

				tag.CategoryName = contract.CategoryName;
				tag.Description = contract.Description;

				var logStr = string.Format("updated properties for {0} ({1})", tag, diff.ChangedFieldsString);
				AuditLog(logStr, session);
				Archive(session, tag, diff, EntryEditEvent.Updated);

				session.Update(tag);

			});

		}

	}

}
