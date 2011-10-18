using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.MikuDb;

namespace VocaDb.Model.Service {

	public class MikuDbAlbumService : ServiceBase {

		public MikuDbAlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public MikuDbAlbumContract[] GetAlbums(AlbumStatus status) {

			return HandleQuery(session => session.Query<MikuDbAlbum>()
				.Where(a => a.Status == status)
				.Select(a => new MikuDbAlbumContract(a))
				.ToArray());

		}

		public int ImportNew() {

			MikuDbAlbumContract[] existing = HandleQuery(session => session.Query<MikuDbAlbum>().Select(a => new MikuDbAlbumContract(a)).ToArray());

			var importer = new AlbumImporter(existing);
			var imported = importer.ImportNew();

			return HandleTransaction(session => {

				var all = session.Query<MikuDbAlbum>();

				//foreach (var album in all)
				//	session.Delete(album);

				var newAlbums = new List<MikuDbAlbum>();

				foreach (var contract in imported) {

					var newAlbum = new MikuDbAlbum(contract.AlbumContract);

					session.Save(newAlbum);

					newAlbums.Add(newAlbum);

				}

				return newAlbums.Count;

			});

		}

	}
}
