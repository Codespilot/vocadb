using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagContract {

		public TagContract() {
			AliasedTo = string.Empty;
		}

		public TagContract(Tag tag) {

			ParamIs.NotNull(() => tag);

			AliasedTo = tag.AliasedTo != null ? tag.AliasedTo.Name : null;
			CategoryName = tag.CategoryName;
			Description = tag.Description;
			Id = tag.Id;
			Name = tag.TagName;
			Status = tag.Status;

		}

		[DataMember]
		public string AliasedTo { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

	}
}
