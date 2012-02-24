using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagContract {

		public TagContract() { }

		public TagContract(Tag tag) {

			ParamIs.NotNull(() => tag);

			CategoryName = tag.CategoryName;
			Description = tag.Description;
			Name = tag.TagName;

		}

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
