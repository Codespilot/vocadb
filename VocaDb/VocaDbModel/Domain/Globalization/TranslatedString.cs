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

		public TranslatedString(TranslatedStringContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			DefaultLanguage = contract.DefaultLanguage;
			English = contract.English;
			Japanese = contract.Japanese;
			Romaji = contract.Romaji;

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

		public string this[ContentLanguagePreference preference] {
			get {
				return
					this[preference == ContentLanguagePreference.Default ? DefaultLanguage : (ContentLanguageSelection)preference];
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

		public virtual IEnumerable<LocalizedString> AllLocalized {
			get {
				return new[] {
					new LocalizedString(Japanese, ContentLanguageSelection.Japanese),
					new LocalizedString(Romaji, ContentLanguageSelection.Romaji), 	
					new LocalizedString(English, ContentLanguageSelection.English), 	
				};
			}
		}

		public virtual string Default {
			get {
				return this[DefaultLanguage];
			}
			set {
				this[DefaultLanguage] = value;
			}
		}

		public virtual string Display {
			get {

				var current = Default;

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
