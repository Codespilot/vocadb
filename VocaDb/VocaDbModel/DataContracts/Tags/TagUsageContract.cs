using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagUsageContract {

		public TagUsageContract() { }

		public TagUsageContract(TagUsage usage) {

			ParamIs.NotNull(() => usage);

			Count = usage.Count;
			TagName = usage.Tag.Name;

		}

		public int Count { get; set; }

		public string TagName { get; set; }

	}
}
