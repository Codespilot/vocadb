using System;
using log4net;
using NHibernate;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service {

	public abstract class ServiceBase {

		private readonly ILog log = LogManager.GetLogger(typeof(ServiceBase));
		private readonly ISessionFactory sessionFactory;
		private readonly IUserPermissionContext permissionContext;

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
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

		protected void DeleteEntity<TEntity>(int id) {

			var typeName = typeof(TEntity).Name;
			log.Info(string.Format("Deleting {0} with Id {1}", typeName, id));

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				session.Delete(entity);

			}, "Unable to delete " + typeName);

		}

		protected void UpdateEntity<TEntity>(int id, Action<TEntity> func) {

			var typeName = typeof(TEntity).Name;

			log.Info(string.Format("Updating {0} with Id {1}", typeName, id));

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);
				func(entity);

				session.Update(entity);

			}, "Unable to update " + typeName);

		}

		protected void UpdateEntity<TEntity>(int id, Action<ISession, TEntity> func) {

			var typeName = typeof(TEntity).Name;

			log.Info(string.Format("Updating {0} with Id {1}", typeName, id));

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);
				func(session, entity);

				session.Update(entity);

			}, "Unable to update " + typeName);

		}

	}

}
