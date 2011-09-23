using NHibernate;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.Helpers {

	public static class SessionHelper {

		public static AgentLoginData CreateAgentLoginData(ISession session, IUserPermissionContext permissionContext) {

			if (permissionContext.LoggedUser != null) {

				var user = session.Load<User>(permissionContext.LoggedUser.Id);
				return new AgentLoginData(user);

			} else {
				
				return new AgentLoginData(permissionContext.Name);

			}			

		}

	}
}
