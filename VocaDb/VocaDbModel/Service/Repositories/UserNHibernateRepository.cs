using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Repositories {

	public class UserNHibernateRepository : NHibernateRepository<User>, IUserRepository {

		public UserNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

	}

}
