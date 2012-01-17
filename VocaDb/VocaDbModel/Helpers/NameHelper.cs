using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class NameHelper {

		public static string GetAdditionalNames(IEnumerable<string> names, string display) {
			return string.Join(", ", names.Where(n => n != display));
		}

		public static EntryName GetName(IEntryWithNames entry, ContentLanguagePreference languagePreference) {

			var primary = entry.Names.SortNames[languagePreference];

			return new EntryName(primary, GetAdditionalNames(entry.Names.AllValues, primary));

		}

	}

}
