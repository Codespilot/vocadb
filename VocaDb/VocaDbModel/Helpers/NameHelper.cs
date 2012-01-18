using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Helpers {

	public static class NameHelper {

		public static string GetAdditionalNames(IEnumerable<string> names, string display) {
			return string.Join(", ", names.Where(n => n != display));
		}

		public static EntryNameContract GetName(INameManager nameManager, ContentLanguagePreference languagePreference) {

			var primary = nameManager.SortNames[languagePreference];

			return new EntryNameContract(primary, GetAdditionalNames(nameManager.AllValues, primary));

		}

	}

}
