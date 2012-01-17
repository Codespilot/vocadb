namespace VocaDb.Model.Domain {

	public class EntryRef {

		public EntryRef() { }

		public EntryRef(IEntryBase entryBase) {

			ParamIs.NotNull(() => entryBase);

			EntryType = entryBase.EntryType;
			Id = entryBase.Id;

		}

		public EntryType EntryType { get; set;  }

		public int Id { get; set; }

		public bool Equals(EntryRef another) {

			if (another == null)
				return false;

			return (EntryType == another.EntryType && Id == another.Id);

		}

		public override bool Equals(object obj) {
			return Equals(obj as EntryRef);
		}

		public override int GetHashCode() {
			return (EntryType.ToString() + "_" + Id).GetHashCode();
		}

	}

}
