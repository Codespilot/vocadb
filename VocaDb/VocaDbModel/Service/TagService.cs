using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Model.Service {

	public class TagService : ServiceBase {

		private Tag GetTag(ISession session, string name) {

			var tag = session.Load<Tag>(name);

			if (name != tag.TagName)
				tag = session.Load<Tag>(tag.TagName);

			return tag;

		}

		public TagService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public void Delete(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var tag = session.Load<Tag>(name);

				AuditLog("deleted " + tag, session);

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

		public string[] FindTags(string query) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] { };

			return HandleQuery(session => {

				string[] tags;

				if (query.Length < 3)
					tags = session.Query<Tag>().Where(t => t.Name == query).Take(10).ToArray().Select(t => t.Name).ToArray();
				else
					tags = session.Query<Tag>().Where(t => t.Name.Contains(query)).Take(10).ToArray().Select(t => t.Name).ToArray();

				return tags;

			});

		}

		public AlbumWithAdditionalNamesContract[] GetAlbums(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session =>
				session.Query<AlbumTagUsage>().Where(a => a.Tag.Name == tagName)
					.Select(t => new AlbumWithAdditionalNamesContract(t.Album, PermissionContext.LanguagePreference)).ToArray());

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

		public ArtistWithAdditionalNamesContract[] GetArtists(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => 
				session.Query<ArtistTagUsage>().Where(a => a.Tag.Name == tagName)
					.Select(t => new ArtistWithAdditionalNamesContract(t.Artist, PermissionContext.LanguagePreference)).ToArray());

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

		public SongWithAdditionalNamesContract[] GetSongs(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session =>
				session.Query<SongTagUsage>().Where(a => a.Tag.Name == tagName)
					.Select(t => new SongWithAdditionalNamesContract(t.Song, PermissionContext.LanguagePreference)).ToArray());

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

				var artists = session.Query<ArtistTagUsage>().Where(a => a.Tag.Name == tagName).OrderByDescending(t => t.Count).Take(15).ToArray();
				var artistCount = session.Query<ArtistTagUsage>().Where(a => a.Tag.Name == tagName).Count();

				var albums = session.Query<AlbumTagUsage>().Where(a => a.Tag.Name == tagName).OrderByDescending(t => t.Count).Take(15).ToArray();
				var albumCount = session.Query<AlbumTagUsage>().Where(a => a.Tag.Name == tagName).Count();

				var songs = session.Query<SongTagUsage>().Where(a => a.Tag.Name == tagName).OrderByDescending(t => t.Count).Take(15).ToArray();
				var songCount = session.Query<SongTagUsage>().Where(a => a.Tag.Name == tagName).Count();

				return new TagDetailsContract(tag, artists, artistCount, albums, albumCount, songs, songCount, 
					PermissionContext.LanguagePreference);
				
			});

		}

		public TagForEditContract GetTagForEdit(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => {

				var inUse = session.Query<ArtistTagUsage>().Any(a => a.Tag.Name == tagName) ||
					session.Query<AlbumTagUsage>().Any(a => a.Tag.Name == tagName) ||
					session.Query<SongTagUsage>().Any(a => a.Tag.Name == tagName);

				var contract = new TagForEditContract(session.Load<Tag>(tagName),
					session.Query<Tag>().Select(t => t.CategoryName).OrderBy(t => t).Distinct().ToArray(), !inUse);

				return contract;

			});

		}

		public string[] GetTagNames() {

			return HandleQuery(session => session.Query<Tag>().OrderBy(t => t.Name).Select(t => t.Name).ToArray());

		}

		public TagCategoryContract[] GetTagsByCategories() {

			return HandleQuery(session => {

				var tags = session.Query<Tag>()
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

		public void UpdateTag(TagDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			HandleTransaction(session => {

				var tag = session.Load<Tag>(contract.Name);

				tag.CategoryName = contract.CategoryName;
				tag.Description = contract.Description;

				AuditLog("updated " + tag, session);

				session.Update(tag);

			});

		}

	}

}
