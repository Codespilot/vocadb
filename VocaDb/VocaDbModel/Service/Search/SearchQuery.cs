using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Service.Search {

	public class SearchQuery {

		private readonly SearchWord[] words;

		public SearchQuery(IEnumerable<SearchWord> words) {
			this.words = words.ToArray();
		}

		public SearchWord[] Words {
			get { return words; }
		}

	}

}
