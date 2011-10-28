using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Globalization {

	public class TranslatedString {

		//private string defaultVal;
		private string english;
		private string original;
		//private string other;
		private string romaji;

		public TranslatedString() {
			DefaultLanguage = ContentLanguageSelection.Japanese;
		}

		/*public TranslatedString(string uniform)
			: this() {

			Japanese = Romaji = English = uniform;

		}*/

		public TranslatedString(TranslatedStringContract contract)
			: this() {

			CopyFrom(contract);

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
				return GetBestMatch(preference);
			}
		}

		/// <summary>
		/// All names in prioritized order.
		/// Cannot be null.
		/// </summary>
		public virtual IEnumerable<string> All {
			get {
				return new[] {
					Japanese,
					Romaji,
					English,
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

		/*public virtual string Default {
			get {  return defaultVal; }
			protected set {
				ParamIs.NotNullOrEmpty(() => value);
				defaultVal = value;
			}
		}*/

		public virtual string Default {
			get {
				
				var val = this[DefaultLanguage];

				return val ?? All.FirstOrDefault(n => !string.IsNullOrEmpty(n));

			}
		}

		public virtual ContentLanguageSelection DefaultLanguage { get; set; }

		public virtual string English {
			get { return english; }
			set {
				english = value;
				//UpdateDefault();
			}
		}

		public virtual string Japanese {
			get { return original; }
			set {
				original = value;
			}
		}

		public virtual string Romaji {
			get { return romaji; }
			set {
				romaji = value;
			}
		}

		public virtual void CopyFrom(TranslatedStringContract contract) {

			ParamIs.NotNull(() => contract);

			DefaultLanguage = contract.DefaultLanguage;
			English = contract.English;
			Japanese = contract.Japanese;
			Romaji = contract.Romaji;

		}

		public virtual string GetBestMatch(ContentLanguagePreference preference) {

			var val = this[preference == ContentLanguagePreference.Default ? DefaultLanguage : (ContentLanguageSelection)preference];

			return (!string.IsNullOrEmpty(val) ? val : Default);

		}

		/*public virtual void UpdateDefault() {

			var val = this[DefaultLanguage];

			if (string.IsNullOrEmpty(val))
				val = All.FirstOrDefault(n => !string.IsNullOrEmpty(n));

			Default = val;

		}*/

	}

}
