namespace VocaDb.Model.Domain {

	public interface IEntryBase : IDeletableEntry {

		string DefaultName { get; }

		EntryType EntryType { get; }

	}

}
