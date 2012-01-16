namespace VocaDb.Model.Domain {

	public class EntryRef {

		public EntryRef(IEntryBase entryBase) {

			ParamIs.NotNull(() => entryBase);

			EntryType = entryBase.EntryType;
			Id = entryBase.Id;

		}

		public EntryType EntryType { get; set;  }

		public int Id { get; set; }

	}

}
