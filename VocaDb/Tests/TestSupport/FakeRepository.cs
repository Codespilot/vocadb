using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeRepository<T> : IRepository<T> {

		private readonly QuerySourceList querySource;

		private ListRepositoryContext<T> CreateContext() {
			return new ListRepositoryContext<T>(querySource);
		}

		public FakeRepository() {
			querySource = new QuerySourceList();
		}
 
		public FakeRepository(params T[] items) {
			querySource = new QuerySourceList();
			querySource.AddRange(items);
		} 

		public FakeRepository(QuerySourceList querySource) {
			this.querySource = querySource;
		}

		public void Add<TEntity>(TEntity entity) {
			querySource.Add(entity);
		}

		public bool Contains<TEntity>(TEntity entity) {
			return querySource.List<TEntity>().Contains(entity);
		}

		public TResult HandleQuery<TResult>(Func<IRepositoryContext<T>, TResult> func, string failMsg) {
			return func(CreateContext());
		}

		public void HandleTransaction(Action<IRepositoryContext<T>> func, string failMsg = "Unexpected database error") {
			func(CreateContext());
		}

		public TResult HandleTransaction<TResult>(Func<IRepositoryContext<T>, TResult> func, string failMsg = "Unexpected database error") {
			return func(CreateContext());
		}

		public List<TEntity> List<TEntity>() {
			return querySource.List<TEntity>();
		}

	}

	public class ListRepositoryContext<T> : IRepositoryContext<T> {

		private readonly QuerySourceList querySource;

		public ListRepositoryContext(QuerySourceList querySource) {
			this.querySource = querySource;
		}

		public IAuditLogger AuditLogger {
			get {
				return new FakeAuditLogger();
			}
		}

		public void Delete(T entity) {
			querySource.List<T>().Remove(entity);
		}

		public void Dispose() {
			
		}

		public T Load(object id) {

			if (!typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)))
				throw new NotSupportedException("Only supported for entities with integer Id");

			var intId = (int)id;
			var list = querySource.List<T>().Cast<IEntryWithIntId>();
			return (T)list.FirstOrDefault(i => i.Id == intId);

		}

		public IRepositoryContext<T2> OfType<T2>() {
			return new ListRepositoryContext<T2>(querySource);
		}

		public IQueryable<T> Query() {
			return querySource.Query<T>();
		}

		public void Save(T obj) {
			querySource.Add(obj);
		}

		public void Update(T obj) {

			if (!typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)))
				throw new NotSupportedException("Only supported for entities with integer Id");

			var entity = (IEntryWithIntId) obj;
			var existing = Load(entity.Id);
			Delete(existing);	// Replace existing
			Save(obj);

		}

	}

}
