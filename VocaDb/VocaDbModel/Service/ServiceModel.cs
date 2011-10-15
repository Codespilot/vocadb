using NHibernate;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service {

	public class ServiceModel {

		private readonly IUserPermissionContext permissionContext;
		private readonly ISessionFactory sessionFactory;

		public ServiceModel(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) {
			this.sessionFactory = sessionFactory;
			this.permissionContext = permissionContext;
		}

		public AdminService Admin {
			get {
				return new AdminService(sessionFactory, permissionContext);
			}
		}

		public AlbumService Albums {
			get {
				return new AlbumService(sessionFactory, permissionContext);
			}
		}

		public ArtistService Artists {
			get {
				return new ArtistService(sessionFactory, permissionContext);
			}
		}

		public MikuDbAlbumService MikuDbAlbums {
			get {
				return new MikuDbAlbumService(sessionFactory, permissionContext);				
			}
		}

		public OtherService Other {
			get {
				return new OtherService(sessionFactory, permissionContext);
			}
		}

		public RankingService Rankings {
			get {
				return new RankingService(sessionFactory, permissionContext);
			}
		}

		public SongService Songs {
			get {
				return new SongService(sessionFactory, permissionContext);
			}
		}

		public UserService Users {
			get {
				return new UserService(sessionFactory, permissionContext);
			}
		}

	}
}
