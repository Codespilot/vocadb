using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	/// <summary>
	/// Fake in-memory repository for testing.
	/// </summary>
	/// <typeparam name="T">Type of entities this repository contains.</typeparam>
	public class FakeRepository<T> : IRepository<T> {

		protected readonly QuerySourceList querySource;

		protected virtual ListRepositoryContext<T> CreateContext() {
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

		/// <summary>
		/// Adds a list of entities to the repository without performing any additional persisting logic.
		/// For example, Ids will not be generated this way.
		/// </summary>
		/// <typeparam name="TEntity">Type of entities to be added.</typeparam>
		/// <param name="entities">List of entities to be added. Cannot be null.</param>
		public void Add<TEntity>(params TEntity[] entities) {
			querySource.AddRange(entities);
		}

		public bool Contains<TEntity>(TEntity entity) {
			return querySource.List<TEntity>().Contains(entity);
		}

		public TResult HandleQuery<TResult>(Func<IRepositoryContext<T>, TResult> func, string failMsg = "Unexpected database error") {
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

		/// <summary>
		/// Save the entity into the repository using the repository's own Save method.
		/// Usually this means an Id will be assigned for the entity, if it's not persisted.
		/// </summary>
		/// <typeparam name="T2">Type of entity to be saved.</typeparam>
		/// <param name="obj">Entity to be saved. Cannot be null.</param>
		public void Save<T2>(T2 obj) {
			CreateContext().Save(obj);
		}

	}

	public class ListRepositoryContext<T> : IRepositoryContext<T> {

		private bool IsEntityWithId {
			get {
				return (typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)));
			}
		}

		protected readonly QuerySourceList querySource;

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

		public virtual T Load(object id) {

			if (!IsEntityWithId)
				throw new NotSupportedException("Only supported for entities with integer Id");

			var intId = (int)id;
			var list = querySource.List<T>().Cast<IEntryWithIntId>().ToArray();

			if (list.All(i => i.Id != intId))
				throw new InvalidOperationException(string.Format("Entity of type {0} with Id {1} not found", typeof(T), id));

			return (T)list.First(i => i.Id == intId);

		}

		public virtual IRepositoryContext<T2> OfType<T2>() {
			return new ListRepositoryContext<T2>(querySource);
		}

		public IQueryable<T> Query() {
			return querySource.Query<T>();
		}

		public void Save(T obj) {

			if (IsEntityWithId) {

				var entity = (IEntryWithIntId)obj;

				// Get next Id
				if (entity.Id == 0) {
					entity.Id = (Query().Any() ? Query().Max(o => ((IEntryWithIntId)o).Id) + 1 : 1);
				}

			}

			querySource.Add(obj);

		}

		public virtual void Update(T obj) {

			if (!IsEntityWithId)
				throw new NotSupportedException("Only supported for entities with integer Id");

			var entity = (IEntryWithIntId) obj;
			var existing = Load(entity.Id);
			Delete(existing);	// Replace existing
			Save(obj);

		}

	}

}
