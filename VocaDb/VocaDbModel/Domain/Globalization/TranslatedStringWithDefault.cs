using System;

namespace VocaDb.Model.Domain.Globalization {

	public class TranslatedStringWithDefault : TranslatedString {

		public new static TranslatedStringWithDefault Create(Func<ContentLanguageSelection, string> factory) {

			return new TranslatedStringWithDefault(
				factory(ContentLanguageSelection.Japanese),
				factory(ContentLanguageSelection.Romaji),
				factory(ContentLanguageSelection.English),
				factory(ContentLanguageSelection.Unspecified)
			);

		}

		private string def;

		public TranslatedStringWithDefault() {}

		public TranslatedStringWithDefault(string original, string romaji, string english, string def)
			: base(original, romaji, english) {

			Default = def;

		}

		public override string this[ContentLanguageSelection language] {
			get {

				switch (language) {
					case ContentLanguageSelection.English:
						return English;
					case ContentLanguageSelection.Japanese:
						return Japanese;
					case ContentLanguageSelection.Romaji:
						return Romaji;
					default:
						return Default;
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
						Default = value;
						break;
				}

			}
		}

		public override string Default {
			get { return def; }
			set {
				ParamIs.NotNull(() => value);
				def = value; 
			}
		}

		public override string GetBestMatch(ContentLanguagePreference preference) {

			return GetBestMatch(preference, ContentLanguageSelection.Unspecified);

		}

		public override string GetDefaultOrFirst() {

			return GetDefaultOrFirst(ContentLanguageSelection.Unspecified);

		}

	}

}
