﻿using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.Repositories {

	public class NHibernateRepositoryContext<T> : IRepositoryContext<T> {

		public IUserPermissionContext PermissionContext { get; private set; }
		public ISession Session { get; private set; }

		public NHibernateRepositoryContext(ISession session, IUserPermissionContext permissionContext) {
			Session = session;
			PermissionContext = permissionContext;
		}

		public IAuditLogger AuditLogger {
			get { return new NHibernateAuditLogger(OfType<AuditLogEntry>(), PermissionContext); }
		}

		public void Delete(T entity) {
			Session.Delete(entity);
		}

		public void Dispose() {
			Session.Dispose();
		}

		public T Load(int id) {
			return Session.Load<T>(id);
		}

		public IRepositoryContext<T2> OfType<T2>() {
			return new NHibernateRepositoryContext<T2>(Session, PermissionContext);
		}

		public IQueryable<T> Query() {
			return Session.Query<T>();
		}

		public void Save(T obj) {
			Session.Save(obj);
		}

		public void Update(T obj) {
			Session.Update(obj);
		}

	}

}