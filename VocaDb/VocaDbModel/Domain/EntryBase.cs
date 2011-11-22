using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain {

	public interface IEntryBase {

		string DefaultName { get; }

		EntryType EntryType { get; }

		int Id { get; }

	}

}
