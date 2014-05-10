using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagForApiContract {

		public TagForApiContract() { }

		public TagForApiContract(Tag tag, 
			IEntryImagePersisterOld thumbPersister,
			bool ssl,			
			TagOptionalFields optionalFields) {
			
			Id = tag.Id;
			Name = tag.Name;

			if (optionalFields.HasFlag(TagOptionalFields.Description)) {
				Description = tag.Description;
			}

			if (optionalFields.HasFlag(TagOptionalFields.MainPicture) && tag.Thumb != null) {
				MainPicture = new EntryThumbForApiContract(tag.Thumb, thumbPersister, ssl);
			}

		}

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

	[Flags]
	public enum TagOptionalFields {

		None = 0,
		Description = 1,
		MainPicture = 2,

	}

}
