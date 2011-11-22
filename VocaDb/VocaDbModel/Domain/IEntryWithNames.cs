using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain {

	public interface IEntryWithNames {

		INameManager Names { get; }

	}
}
