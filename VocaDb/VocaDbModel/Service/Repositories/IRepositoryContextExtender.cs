using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Repositories {

	/// <summary>
	/// Extension methods for <see cref="IRepositoryContext"/>.
	/// </summary>
	public static class IRepositoryContextExtender {

		public static AgentLoginData CreateAgentLoginData<T>(this IRepositoryContext<T> ctx, IUserPermissionContext permissionContext, User user = null) {

			if (user != null)
				return new AgentLoginData(user);

			if (permissionContext.IsLoggedIn) {

				user = ctx.OfType<User>().GetLoggedUser(permissionContext);
				return new AgentLoginData(user);

			} else {

				return new AgentLoginData(permissionContext.Name);

			}

		}

		public static User GetLoggedUser(this IRepositoryContext<User> ctx, IUserPermissionContext permissionContext) {

			permissionContext.VerifyLogin();

			return ctx.Load(permissionContext.LoggedUserId);

		}

		public static void Sync<T>(this IRepositoryContext<T> ctx, CollectionDiff<T, T> diff) {

			ParamIs.NotNull(() => ctx);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				ctx.Delete(n);

			foreach (var n in diff.Added)
				ctx.Save(n);

			foreach (var n in diff.Unchanged)
				ctx.Update(n);

		}

		public static void Sync<T>(this IRepositoryContext<T> ctx, CollectionDiffWithValue<T, T> diff) {

			ParamIs.NotNull(() => ctx);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				ctx.Delete(n);

			foreach (var n in diff.Added)
				ctx.Save(n);

			foreach (var n in diff.Edited)
				ctx.Update(n);

		}


	}

}
