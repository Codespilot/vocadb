namespace VocaDb.Model.Domain.Globalization {

	public class LocalizedString {

		private string val;

		public LocalizedString() {
			Language = ContentLanguageSelection.Japanese;
			Value = string.Empty;
		}

		public LocalizedString(string val, ContentLanguageSelection language) 
			: this() {

			Value = val;
			Language = language;

		}

		public virtual ContentLanguageSelection Language { get; private set; }

		public virtual string Value {
			get { return val; }
			set {
				ParamIs.NotNull(() => value);
				val = value;
			}
		}

	}
}
