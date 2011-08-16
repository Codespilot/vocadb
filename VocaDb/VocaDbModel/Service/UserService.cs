using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Security;
using VocaVoter.Model.Service.Security;

namespace VocaDb.Model.Service {

	public class UserService : ServiceBase {

		public UserService(ISessionFactory sessionFactory)
			: base(sessionFactory) {

		}

		public bool CheckAuthentication(string email, string pass) {

			return HandleQuery(session => {

				var lc = email.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Email == lc);

				if (user == null)
					return false;

				var hashed = LoginManager.GetHashedPass(lc, pass, user.Salt);

				return (user.Password == hashed);

			});

		}

		/*public UserContract GetUser(int userId) {

			return HandleQuery(session => session.Linq<User>());

		}*/

	}

}
