using System.Collections.Generic;
using NHibernate;

namespace VocaDb.Model.Service.Search {

	public interface ISearchFilter<TEntry> {

		QueryCost Cost { get; }

		void FilterResults(List<TEntry> albums, ISession session);

		List<TEntry> GetResults(ISession session);

	}

}
