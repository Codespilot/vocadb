using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class LocalizedStringHelper {

		public static IEnumerable<LocalizedStringContract> SkipNull(string original, string romaji, string english) {

			var names = new List<LocalizedStringContract>();

			if (!string.IsNullOrEmpty(original))
				names.Add(new LocalizedStringContract(original, ContentLanguageSelection.Japanese));

			if (!string.IsNullOrEmpty(romaji))
				names.Add(new LocalizedStringContract(romaji, ContentLanguageSelection.Romaji));

			if (!string.IsNullOrEmpty(english))
				names.Add(new LocalizedStringContract(english, ContentLanguageSelection.English));

			return names;

		}

	}
}
