using System.Runtime.Serialization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Service.Paging {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PagingProperties {

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
