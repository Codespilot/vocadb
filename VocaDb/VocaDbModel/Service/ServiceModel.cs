using NHibernate;

namespace VocaDb.Model.Service {

	public class ServiceModel {

		private readonly ISessionFactory sessionFactory;

		public ServiceModel(ISessionFactory sessionFactory) {
			this.sessionFactory = sessionFactory;
		}

		public AlbumService Albums {
			get {
				return new AlbumService(sessionFactory);
			}
		}

		public ArtistService Artists {
			get {
				return new ArtistService(sessionFactory);
			}
		}

		public RankingService Rankings {
			get {
				return new RankingService(sessionFactory);
			}
		}

		public SongService Songs {
			get {
				return new SongService(sessionFactory);
			}
		}

		public UserService Users {
			get {
				return new UserService(sessionFactory);
			}
		}

	}
}
