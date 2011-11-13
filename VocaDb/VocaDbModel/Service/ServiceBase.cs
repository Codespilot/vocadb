using System;
using System.Linq;
using log4net;
using NHibernate;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using System.Collections.Generic;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.Service {

	public abstract class ServiceBase {

		private readonly ILog log = LogManager.GetLogger(typeof(ServiceBase));
		private readonly ISessionFactory sessionFactory;
		private readonly IUserPermissionContext permissionContext;

		private string GetAuditLogMessage(string doingWhat) {

			return string.Format("'{0}' {1}", PermissionContext.Name, doingWhat);

		}

		protected User GetLoggedUser(ISession session) {

			if (!PermissionContext.IsLoggedIn)
				throw new InvalidOperationException("Must be logged in");

			return session.Load<User>(PermissionContext.LoggedUser.Id);

		}

		protected User GetLoggedUserOrDefault(ISession session) {

			return (PermissionContext.LoggedUser != null ? session.Load<User>(PermissionContext.LoggedUser.Id) : null);

		}

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		protected ServiceModel Services {
			get {
				return new ServiceModel(sessionFactory, PermissionContext);
			}
		}

		protected void AuditLog(string doingWhat) {

			log.Info(GetAuditLogMessage(doingWhat));

		}

		protected void AuditLog(string doingWhat, ISession session, User user = null) {

			ParamIs.NotNull(() => session);

			AuditLog(doingWhat);

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext, user);
			var entry = new AuditLogEntry(agentLoginData, doingWhat);

			session.Save(entry);

		}

		protected bool DoSnapshot(ArchivedObjectVersion latestVersion) {

			if (latestVersion == null)
				return true;

			return ((((latestVersion.Version + 1) % 5) == 0) || DateTime.Now - latestVersion.Created >= TimeSpan.FromDays(7));

		}

		protected T HandleQuery<T>(Func<ISession, T> func, string failMsg = "Unexpected NHibernate error") {
			
			try {
				using (var session = OpenSession()) {
					return func(session);
				}
			} catch (HibernateException x) {
				log.Error(failMsg, x);
			throw;
			}

		}

		protected T HandleTransaction<T>(Func<ISession, T> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction()) {

					var val = func(session);
					tx.Commit();
					return val;

				}
			} catch (HibernateException x) {
				log.Error(failMsg, x);
				throw;
			}

		}

		protected void HandleTransaction(Action<ISession> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction()) {

					func(session);
					tx.Commit();

				}
			} catch (HibernateException x) {
				log.Error(failMsg, x);
				throw;
			}

		}

		protected ISession OpenSession() {
			return sessionFactory.OpenSession();
		}

		protected ServiceBase(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) {
			ParamIs.NotNull(() => sessionFactory);
			this.sessionFactory = sessionFactory;
			this.permissionContext = permissionContext;
		}

		protected void DeleteEntity<TEntity>(int id, PermissionFlags permissionFlags = PermissionFlags.Nothing, bool skipLog = false) {

			var typeName = typeof(TEntity).Name;
			AuditLog(string.Format("is about to delete {0} with Id {1}", typeName, id));
			PermissionContext.VerifyPermission(permissionFlags);

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					AuditLog("deleting " + entity, session);
				else
					AuditLog("deleting " + entity);

				session.Delete(entity);

			}, "Unable to delete " + typeName);

		}

		protected void UpdateEntity<TEntity>(int id, Action<TEntity> func, PermissionFlags permissionFlags = PermissionFlags.Nothing, bool skipLog = false) {

			var typeName = typeof(TEntity).Name;

			AuditLog(string.Format("is about to update {0} with Id {1}", typeName, id));
			PermissionContext.VerifyPermission(permissionFlags);

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					AuditLog("updating " + entity, session);
				else
					AuditLog("updating " + entity);

				func(entity);

				session.Update(entity);

			}, "Unable to update " + typeName);

		}

		protected void UpdateEntity<TEntity>(int id, Action<ISession, TEntity> func, PermissionFlags permissionFlags = PermissionFlags.Nothing, bool skipLog = false) {

			var typeName = typeof(TEntity).Name;

			AuditLog(string.Format("is about to update {0} with Id {1}", typeName, id));
			PermissionContext.VerifyPermission(permissionFlags);

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					AuditLog("updating " + entity, session);
				else
					AuditLog("updating " + entity);

				func(session, entity);

				session.Update(entity);

			}, "Unable to update " + typeName);

		}

		protected void VerifyResourceAccess(params User[] owners) {

			VerifyResourceAccess(owners.Select(o => o.Id));
				
		}

		protected void VerifyResourceAccess(params UserContract[] owners) {

			VerifyResourceAccess(owners.Select(o => o.Id));

		}

		private void VerifyResourceAccess(IEnumerable<int> ownerIds) {

			if (!PermissionContext.IsLoggedIn)
				throw new NotAllowedException("Must be logged in.");

			if (!ownerIds.Contains(PermissionContext.LoggedUser.Id))
				throw new NotAllowedException("You do not have access to this resource.");

		}


	}

	/// <summary>
	/// Match mode for name queries.
	/// </summary>
	public enum NameMatchMode {

		/// <summary>
		/// Automatically choose match mode based on query term length
		/// </summary>
		Auto,

		/// <summary>
		/// Always partial matching
		/// </summary>
		Partial,

		/// <summary>
		/// Always exact matching (still case-insensitive)
		/// </summary>
		Exact

	}

	public class PartialFindResult<T> {

		public PartialFindResult(T[] items, int totalCount) {

			Items = items;
			TotalCount = totalCount;

		}

		public T[] Items { get; set; }

		public int TotalCount { get; set; }

	}


}
