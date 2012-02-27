using NHibernate;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service {

	public class ServiceModel {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IUserPermissionContext permissionContext;
		private readonly ISessionFactory sessionFactory;

		public ServiceModel(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) {
			this.sessionFactory = sessionFactory;
			this.permissionContext = permissionContext;
			this.entryLinkFactory = entryLinkFactory;
		}

		public AdminService Admin {
			get {
				return new AdminService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public AlbumService Albums {
			get {
				return new AlbumService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public ArtistService Artists {
			get {
				return new ArtistService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public MikuDbAlbumService MikuDbAlbums {
			get {
				return new MikuDbAlbumService(sessionFactory, permissionContext, entryLinkFactory);				
			}
		}

		public NewsEntryService NewsEntry {
			get {
				return new NewsEntryService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public OtherService Other {
			get {
				return new OtherService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public RankingService Rankings {
			get {
				return new RankingService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public SongService Songs {
			get {
				return new SongService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public TagService Tags {
			get {
				return new TagService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public UserService Users {
			get {
				return new UserService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

	}
}
