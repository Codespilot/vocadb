using System;
using System.Linq;

namespace VocaDb.Model.Service.Repositories {

	/// <summary>
	/// Interface for an unit of work against the database.
	/// </summary>
	/// <typeparam name="T">Type of entity.</typeparam>
	public interface IRepositoryContext<T> : IDisposable {

		/// <summary>
		/// Audit logger for the database.
		/// </summary>
		IAuditLogger AuditLogger { get; }

		/// <summary>
		/// Loads an entity from the database.
		/// </summary>
		/// <param name="id">Entity Id.</param>
		/// <returns>The loaded entity. Cannot be null.</returns>
		T Load(int id);

		/// <summary>
		/// Returns a child context for another entity type.
		/// The unit of work (including transaction) must be shared between this parent context and the child context.
		/// </summary>
		/// <typeparam name="T2">New entity type.</typeparam>
		/// <returns>Child context for that entity type. Cannot be null.</returns>
		IRepositoryContext<T2> OfType<T2>();
			
		/// <summary>
		/// LINQ query against the database.
		/// </summary>
		/// <returns>Queryable interface. Cannot be null.</returns>
		IQueryable<T> Query();

		/// <summary>
		/// Persists a new entity in the repository.
		/// </summary>
		/// <param name="obj">Entity to be saved. Cannot be null.</param>
		void Save(T obj);

		/// <summary>
		/// Updates an existing entity in the repository.
		/// </summary>
		/// <param name="obj">Entity to be updated. Cannot be null.</param>
		void Update(T obj);

	}
}