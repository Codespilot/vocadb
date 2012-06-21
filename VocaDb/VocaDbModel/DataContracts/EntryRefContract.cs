using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

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

		public EntryType EntryType { get; set; }

		public int Id { get; set; }

	}

}
