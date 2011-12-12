using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain {

	public interface IEntryBase : IEntryWithIntId {

		string DefaultName { get; }

		EntryType EntryType { get; }

	}

}
