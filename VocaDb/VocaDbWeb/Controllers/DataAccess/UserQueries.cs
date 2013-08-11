using System;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="User"/>.
	/// </summary>
	public class UserQueries {

		private readonly IUserRepository repository;

		private string MakeGeoIpToolLink(string hostname) {

			return string.Format("<a href='http://www.geoiptool.com/?IP={0}'>{0}</a>", hostname);

		}

		public UserQueries(IUserRepository repository) {
			this.repository = repository;
		}

		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="pass">Password. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique. Cannot be null.</param>
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <param name="timeSpan">Time in which the user filled the registration form.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		public UserContract Create(string name, string pass, string email, string hostname, TimeSpan timeSpan) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			return repository.HandleTransaction(ctx => {

				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					existing = ctx.Query().FirstOrDefault(u => u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				var salt = new Random().Next();
				var hashed = LoginManager.GetHashedPass(lc, pass, salt);
				var user = new User(name, hashed, email, salt);
				user.UpdateLastLogin(hostname);
				ctx.Save(user);

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} in {1}.", MakeGeoIpToolLink(hostname), timeSpan), user);

				return new UserContract(user);

			});

		}

	}

}