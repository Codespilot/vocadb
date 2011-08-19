using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.Service {

	public class UserService : ServiceBase {

		public UserService(ISessionFactory sessionFactory)
			: base(sessionFactory) {

		}

		public UserContract CheckAuthentication(string name, string pass) {

			return HandleQuery(session => {

				var lc = name.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Name == lc);

				if (user == null)
					return null;

				var hashed = LoginManager.GetHashedPass(lc, pass, user.Salt);

				if (user.Password != hashed)
					return null;

				return new UserContract(user);

			});

		}

		public UserContract Create(string name, string pass) {

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();
				var existing = session.Query<User>().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					return null;

				var salt = new Random().Next();
				var hashed = LoginManager.GetHashedPass(lc, pass, salt);
				var user = new User(name, hashed, salt);
				session.Save(user);

				return new UserContract(user);

			});

		}

		public UserContract[] GetUsers() {

			return HandleQuery(session => session.Query<User>().Select(u => new UserContract(u)).ToArray());

		}

		/*public UserContract GetUser(int userId) {

			return HandleQuery(session => session.Linq<User>());

		}*/

	}

}
