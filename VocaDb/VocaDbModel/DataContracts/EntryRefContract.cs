using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryRefContract {

		public EntryRefContract() { }

		public EntryRefContract(IEntryBase entryBase) {

			ParamIs.NotNull(() => entryBase);

			EntryType = entryBase.EntryType;
			Id = entryBase.Id;

		}

		public EntryRefContract(EntryRef entryRef) {

			ParamIs.NotNull(() => entryRef);

			EntryType = entryRef.EntryType;
			Id = entryRef.Id;

		}

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public int Id { get; set; }

	}

}
