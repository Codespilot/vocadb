﻿using VocaDb.Model.Domain;
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

		public static void Delete<T, T2>(this IRepositoryContext<T> ctx, T2 obj) {
			ctx.OfType<T2>().Delete(obj);
		}

		public static User GetLoggedUser(this IRepositoryContext<User> ctx, IUserPermissionContext permissionContext) {

			permissionContext.VerifyLogin();

			return ctx.Load(permissionContext.LoggedUserId);

		}

		public static T2 Load<T, T2>(this IRepositoryContext<T> ctx, object id) {
			return ctx.OfType<T2>().Load(id);
		}

		public static T NullSafeLoad<T>(this IRepositoryContext<T> ctx, IEntryWithIntId entry) {
			return entry != null && entry.Id != 0 ? ctx.Load(entry.Id) : default(T);
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

		public static void Save<T, T2>(this IRepositoryContext<T> ctx, T2 obj) {
			ctx.OfType<T2>().Save(obj);
		}

		public static void Update<T, T2>(this IRepositoryContext<T> ctx, T2 obj) {
			ctx.OfType<T2>().Update(obj);
		}

	}

}
