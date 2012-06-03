namespace VocaDb.Model.Service.Paging {

	public class PagingProperties {

		public static PagingProperties CreateFromPage(int page, int entriesPerPage, bool getTotalCount) {

			return new PagingProperties(page * entriesPerPage, entriesPerPage, getTotalCount);

		}

		public PagingProperties(int start, int maxEntries, bool getTotalCount) {
			Start = start;
			MaxEntries = maxEntries;
			GetTotalCount = getTotalCount;
		}

		public bool GetTotalCount { get; private set; }

		public int MaxEntries { get; private set; }

		public int Start { get; private set; }

	}
}
