using System;
using System.Collections.Generic;
using System.Linq;
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

		public void Dispose() {
			
		}

		public T Load(int id) {
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

	}

}
