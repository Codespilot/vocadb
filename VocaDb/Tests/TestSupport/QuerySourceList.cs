using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Service.Search;

namespace VocaDb.Tests.TestSupport {

	public class QuerySourceList : IQuerySource {

		private readonly Dictionary<Type, IList> entities;

		public QuerySourceList() {
			entities = new Dictionary<Type, IList>();
		}

		public void Add<TEntity>(TEntity entity) {
			List<TEntity>().Add(entity);
		}

		public void AddRange<TEntity>(params TEntity[] entities) {
			List<TEntity>().AddRange(entities);
		}

		public List<TEntity> List<TEntity>() {

			var t = typeof(TEntity);

			if (!entities.ContainsKey(t))
				entities.Add(t, new List<TEntity>());

			return (List<TEntity>)entities[t];

		}

		public T Load<T>(object id) {
			return default(T);
		}

		public IQueryable<T> Query<T>() {
		
			var t = typeof(T);

			if (entities.ContainsKey(t))
				return ((List<T>)entities[t]).AsQueryable();
			else
				return (new T[] { }).AsQueryable();

		}

	}
}
