using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Globalization {

	public class NameManager<T> where T : LocalizedStringWithId {

		private IList<T> names = new List<T>();
		private TranslatedString sortNames = new TranslatedString();

		private T GetFirstName(ContentLanguageSelection languageSelection) {

			if (!Names.Any())
				return null;

			var name = Names.FirstOrDefault(n => n.Language == languageSelection);

			if (name == null)
				name = Names.FirstOrDefault(n => n.Language == ContentLanguageSelection.Unspecified);

			return name;

		}

		private void SetValueFor(ContentLanguageSelection language) {

			if (!Names.Any())
				return;

			var name = GetFirstName(language);

			if (name != null)
				SortNames[language] = name.Value;

			if (string.IsNullOrEmpty(SortNames[language]))
				SortNames[language] = Names.First().Value;

		}

		public virtual IEnumerable<string> AllValues {
			get {

				return SortNames.All
					.Concat(Names.Select(n => n.Value))
					.Distinct();

			}
		}

		public virtual IList<T> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual TranslatedString SortNames {
			get { return sortNames; }
			set {
				ParamIs.NotNull(() => value);
				sortNames = value;
			}
		}

		public virtual bool HasNameFor(ContentLanguageSelection language) {

			return Names.Any(n => n.Language == language || n.Language == ContentLanguageSelection.Unspecified);

		}

		public virtual void UpdateSortNames() {

			if (!Names.Any())
				return;

			var languages = new[] { ContentLanguageSelection.Japanese, ContentLanguageSelection.Romaji, ContentLanguageSelection.English };

			foreach (var l in languages)
				SetValueFor(l);		

		}

	}
}
