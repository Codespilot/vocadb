using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public ActivityEntryContract GetActivityEntries(int maxEntries) {

			return HandleQuery(session => {

				var entries = session.Query<ActivityEntry>().OrderByDescending(a => a.CreateDate).Take(maxEntries);

				return new ActivityEntryContract(null);

			});

		}

		public FrontPageContract GetFrontPageContent() {

			return HandleQuery(session => new FrontPageContract(
				session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.CreateDate).Take(15),
				session.Query<Song>().Where(s => !s.Deleted)
					.OrderByDescending(s => s.CreateDate).Take(15), 
				PermissionContext.LanguagePreference));

		}

	}
}
