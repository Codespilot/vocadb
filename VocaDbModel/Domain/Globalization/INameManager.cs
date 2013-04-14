using System.Collections.Generic;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Globalization {

	public interface INameManager {

		IEnumerable<string> AllValues { get; }

		IEnumerable<LocalizedStringWithId> NamesBase { get; }

		TranslatedString SortNames { get; }

		LocalizedStringWithId FirstNameBase(ContentLanguageSelection languageSelection);

		EntryNameContract GetEntryName(ContentLanguagePreference languagePreference);

		bool HasNameForLanguage(ContentLanguageSelection language);

	}

}
