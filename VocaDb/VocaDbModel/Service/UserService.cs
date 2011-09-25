using System;
using System.Linq;
using System.Runtime.Serialization;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.Service {

	public class UserService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(UserService));

		public UserService(ISessionFactory sessionFactory)
			: base(sessionFactory) {

		}

		public UserContract CheckAuthentication(string name, string pass) {

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.Name == lc);

				if (user == null)
					return null;

				var hashed = LoginManager.GetHashedPass(lc, pass, user.Salt);

				if (user.Password != hashed)
					return null;

				log.Info("'" + user + "' logged in");

				user.UpdateLastLogin();
				session.Update(user);

				return new UserContract(user);

			});

		}

		public UserContract Create(string name, string pass) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);

			log.Info("Creating user '" + name + "'");

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

		public UserContract GetUser(int id) {

			return HandleQuery(session => new UserContract(session.Load<User>(id)));

		}

		public UserContract GetUserByName(string name) {

			return HandleQuery(session => new UserContract(session.Query<User>().First(u => u.Name == name)));

		}

		public UserContract UpdateUserSettings(UpdateUserSettingsContract contract, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => contract);
			ParamIs.NotNull(() => permissionContext);

			return HandleTransaction(session => {

				var user = session.Load<User>(contract.Id);

				if (!string.IsNullOrEmpty(contract.NewPass)) {

					var oldHashed = LoginManager.GetHashedPass(user.NameLC, contract.OldPass, user.Salt);

					if (user.Password != oldHashed)
						throw new InvalidPasswordException();

					var newHashed = LoginManager.GetHashedPass(user.NameLC, contract.NewPass, user.Salt);
					user.Password = newHashed;

				}

				user.SetEmail(contract.Email);
				session.Update(user);

				return new UserContract(user);

			});

		}

	}

	public class InvalidPasswordException : Exception {
		
		public InvalidPasswordException()
			: base("Invalid password") {}

		protected InvalidPasswordException(SerializationInfo info, StreamingContext context) 
			: base(info, context) {}

	}

}
