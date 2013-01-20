using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Entry reference with localized entry title.
	/// </summary>
	public class EntryRefWithNameContract : EntryRefContract {

		public EntryRefWithNameContract(IEntryWithNames entry, ContentLanguagePreference languagePreference)
			: base(entry) {

			Name = entry.Names.GetEntryName(languagePreference);

		}

		public EntryNameContract Name { get; set; }

	}

}
