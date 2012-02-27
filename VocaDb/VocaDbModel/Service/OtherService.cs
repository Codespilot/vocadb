using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public ActivityEntryContract[] GetActivityEntries(int maxEntries) {

			return HandleQuery(session => {

				var entries = session.Query<ActivityEntry>().OrderByDescending(a => a.CreateDate).Take(maxEntries).ToArray();

				var contracts = entries.Select(e => new ActivityEntryContract(e, PermissionContext.LanguagePreference)).ToArray();

				return contracts;

			});

		}

		public FrontPageContract GetFrontPageContent() {

			const int maxNewsEntries = 4;
			const int maxActivityEntries = 25;

			return HandleQuery(session => {

				var activityEntries = session.Query<ActivityEntry>().OrderByDescending(a => a.CreateDate).Take(maxActivityEntries).ToArray();
				var newsEntries = session.Query<NewsEntry>().Where(n => n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries).ToArray();

				if (newsEntries.Length < maxNewsEntries)
					newsEntries = newsEntries.Concat(session.Query<NewsEntry>().Where(n => !n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries - newsEntries.Length)).ToArray();

				return new FrontPageContract(activityEntries, newsEntries, PermissionContext.LanguagePreference);

			});

		}

	}
}
