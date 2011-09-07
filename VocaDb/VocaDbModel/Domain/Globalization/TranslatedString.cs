using System.Collections.Generic;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Globalization {

	public class TranslatedString {

		public TranslatedString() {
			DefaultLanguage = ContentLanguageSelection.Japanese;
		}

		public TranslatedString(string uniform)
			: this() {

			Japanese = Romaji = English = uniform;

		}

		public string this[ContentLanguageSelection language] {
			get {
				
				switch (language) {
					case ContentLanguageSelection.English:
						return English;
					case ContentLanguageSelection.Japanese:
						return Japanese;
					case ContentLanguageSelection.Romaji:
						return Romaji;
					default:
						return Japanese;
				}

			}
			set {

				switch (language) {
					case ContentLanguageSelection.English:
						English = value;
						break;
					case ContentLanguageSelection.Japanese:
						Japanese = value;
						break;
					case ContentLanguageSelection.Romaji:
						Romaji = value;
						break;
					default:
						Japanese = value;
						break;
				}

			}
		}

		public virtual IEnumerable<string> All {
			get {
				return new[] {
					Japanese,
					Romaji,
					English
				};
			}
		}

		public virtual string Current {
			get {
				return this[DefaultLanguage];
			}
			set {
				this[DefaultLanguage] = value;
			}
		}

		public virtual string Display {
			get {

				var current = Current;

				if (string.IsNullOrEmpty(current))
					return Japanese;
				else
					return current;

			}
		}

		public virtual ContentLanguageSelection DefaultLanguage { get; set; }

		public virtual string English { get; set; }

		public virtual string Japanese { get; set; }

		public virtual string Romaji { get; set; }

		public void CopyFrom(TranslatedStringContract contract) {

			ParamIs.NotNull(() => contract);

			DefaultLanguage = contract.DefaultLanguage;
			English = contract.English;
			Japanese = contract.Japanese;
			Romaji = contract.Romaji;

		}

	}

}
