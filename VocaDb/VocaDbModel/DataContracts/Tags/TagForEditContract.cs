using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagForEditContract : TagContract {

		public TagForEditContract(Tag tag, IEnumerable<string> allCategoryNames)
			: base(tag) {

			AllCategoryNames = allCategoryNames.ToArray();

		}

		public string[] AllCategoryNames { get; set; }

	}
}
