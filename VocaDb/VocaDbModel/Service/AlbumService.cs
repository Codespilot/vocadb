using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) : base(sessionFactory, permissionContext) {}

		public AlbumContract[] Find(string query, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Album>()
					.Where(s => string.IsNullOrEmpty(query)
						|| s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
								|| s.TranslatedName.Japanese.Contains(query))
					.OrderBy(s => s.TranslatedName.Japanese)
					.Take(maxResults)
					.ToArray();

				return direct
					.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public AlbumDetailsContract GetAlbumDetails(int id) {

			return HandleQuery(session => new AlbumDetailsContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}


	}

}
