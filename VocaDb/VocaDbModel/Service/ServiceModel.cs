using NHibernate;

namespace VocaDb.Model.Service {

	public class ServiceModel {

		private readonly ISessionFactory sessionFactory;

		public ServiceModel(ISessionFactory sessionFactory) {
			this.sessionFactory = sessionFactory;
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

	}
}
