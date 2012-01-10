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
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service {

	public class TagService : ServiceBase {

		public TagService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

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

		public ArtistWithAdditionalNamesContract[] GetArtists(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => 
				session.Query<ArtistTagUsage>().Where(a => a.Tag.Name == tagName)
					.Select(t => new ArtistWithAdditionalNamesContract(t.Artist, PermissionContext.LanguagePreference)).ToArray());

		}

		public SongWithAdditionalNamesContract[] GetSongs(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session =>
				session.Query<SongTagUsage>().Where(a => a.Tag.Name == tagName)
					.Select(t => new SongWithAdditionalNamesContract(t.Song, PermissionContext.LanguagePreference)).ToArray());

		}

		public TagDetailsContract GetTagDetails(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => new TagDetailsContract(session.Load<Tag>(tagName), PermissionContext.LanguagePreference));

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

	}

}
