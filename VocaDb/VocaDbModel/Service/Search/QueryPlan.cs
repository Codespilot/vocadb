using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Service.Search {

	public class QueryPlan {

		private readonly SearchWord[] words;

		public QueryPlan(IEnumerable<SearchWord> words) {
			this.words = words.ToArray();
		}

		public SearchWord[] Words {
			get { return words; }
		}

	}

}
