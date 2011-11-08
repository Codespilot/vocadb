using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class LocalizedStringHelper {

		public static IEnumerable<LocalizedStringContract> SkipNullAndEmpty(string original, string romaji, string english) {

			var names = new List<LocalizedStringContract>();

			if (!string.IsNullOrWhiteSpace(original))
				names.Add(new LocalizedStringContract(original.Trim(), ContentLanguageSelection.Japanese));

			if (!string.IsNullOrWhiteSpace(romaji))
				names.Add(new LocalizedStringContract(romaji.Trim(), ContentLanguageSelection.Romaji));

			if (!string.IsNullOrWhiteSpace(english))
				names.Add(new LocalizedStringContract(english.Trim(), ContentLanguageSelection.English));

			return names;

		}

	}
}
