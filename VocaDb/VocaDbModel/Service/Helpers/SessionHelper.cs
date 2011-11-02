using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Helpers {

	public static class SessionHelper {

		public static AgentLoginData CreateAgentLoginData(ISession session, IUserPermissionContext permissionContext, User user = null) {

			if (user != null)
				return new AgentLoginData(user);

			if (permissionContext.LoggedUser != null) {

				user = session.Load<User>(permissionContext.LoggedUser.Id);
				return new AgentLoginData(user);

			} else {
				
				return new AgentLoginData(permissionContext.Name);

			}			

		}

		public static void Sync<T>(ISession session, CollectionDiff<T,T> diff) {

			ParamIs.NotNull(() => session);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				session.Delete(n);

			foreach (var n in diff.Added)
				session.Save(n);

			foreach (var n in diff.Unchanged)
				session.Update(n);

		}

		public static void Sync<T>(ISession session, CollectionDiffWithValue<T, T> diff) {

			ParamIs.NotNull(() => session);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				session.Delete(n);

			foreach (var n in diff.Added)
				session.Save(n);

			foreach (var n in diff.Edited)
				session.Update(n);

		}

	}
}
