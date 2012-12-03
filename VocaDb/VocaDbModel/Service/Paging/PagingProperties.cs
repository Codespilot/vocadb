using System.Runtime.Serialization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Service.Paging {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PagingProperties {

		/// <summary>
		/// Creates paging properties based on a page number (instead of absolute entry index).
		/// </summary>
		/// <param name="page">Page number (starting from 0)</param>
		/// <param name="entriesPerPage">Number of entries per page.</param>
		/// <param name="getTotalCount">Whether to get total count.</param>
		/// <returns>Paging properties. Cannot be null.</returns>
		public static PagingProperties CreateFromPage(int page, int entriesPerPage, bool getTotalCount) {

			return new PagingProperties(page * entriesPerPage, entriesPerPage, getTotalCount);

		}

		public PagingProperties(int start, int maxEntries, bool getTotalCount) {
			Start = start;
			MaxEntries = maxEntries;
			GetTotalCount = getTotalCount;
		}

		[DataMember]
		public bool GetTotalCount { get; set; }

		[DataMember]
		public int MaxEntries { get; set; }

		[DataMember]
		public int Start { get; set; }

	}
}
