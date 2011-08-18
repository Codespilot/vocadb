using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		public AlbumService(ISessionFactory sessionFactory) : base(sessionFactory) {}

		public AlbumDetailsContract GetAlbumDetails(int id) {

			return HandleQuery(session => new AlbumDetailsContract(session.Load<Album>(id)));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new AlbumContract(a))
				.ToArray());

		}


	}

}
