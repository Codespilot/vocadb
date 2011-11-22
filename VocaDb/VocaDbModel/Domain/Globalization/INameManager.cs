using System.Collections.Generic;

namespace VocaDb.Model.Domain.Globalization {

	public interface INameManager {

		IEnumerable<LocalizedStringWithId> Names { get; }

		TranslatedString SortNames { get; }

	}

}
