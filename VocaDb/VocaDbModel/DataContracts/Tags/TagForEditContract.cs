using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagForEditContract : TagContract {

		public TagForEditContract(Tag tag, IEnumerable<string> allCategoryNames, bool isEmpty)
			: base(tag) {

			AllCategoryNames = allCategoryNames.ToArray();
			IsEmpty = isEmpty;

		}

		public string[] AllCategoryNames { get; set; }

		public bool IsEmpty { get; set; }

	}
}
