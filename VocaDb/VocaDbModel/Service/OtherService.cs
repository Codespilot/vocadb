using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public FrontPageContract GetFrontPageContent() {

			return HandleQuery(session => new FrontPageContract(
				session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.CreateDate).Take(20),
				session.Query<Song>().Where(s => !s.Deleted)
					.OrderByDescending(s => s.CreateDate).Take(20), 
				PermissionContext.LanguagePreference));

		}

	}
}
