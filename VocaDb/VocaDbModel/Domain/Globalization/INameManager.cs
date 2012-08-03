using System.Collections.Generic;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Globalization {

	public interface INameManager {

		IEnumerable<string> AllValues { get; }

		//IEnumerable<LocalizedStringWithId> Names { get; }

		TranslatedString SortNames { get; }

		EntryNameContract GetEntryName(ContentLanguagePreference languagePreference);

	}

}
