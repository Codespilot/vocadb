using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Tags;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagUsageContract {

		public TagUsageContract() { }

		public TagUsageContract(TagUsage usage) {

			ParamIs.NotNull(() => usage);

			Count = usage.Count;
			TagName = usage.Tag.Name;

		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public string TagName { get; set; }

	}
}
