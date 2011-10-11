using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class AdminService : ServiceBase {

		public AdminService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public void UpdateArtistStrings() {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			AuditLog("rebuilding artist strings");

			HandleTransaction(session => {

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {
					album.UpdateArtistString();
					session.Update(album);
				}

				var songs = session.Query<Song>().Where(s => !s.Deleted).ToArray();

				foreach (var song in songs) {
					song.UpdateArtistString();
					session.Update(song);
				}

			});

		}

	}
}
