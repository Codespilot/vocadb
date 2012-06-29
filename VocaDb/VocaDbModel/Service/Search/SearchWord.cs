namespace VocaDb.Model.Service.Search {

	public class SearchWord {

		private readonly string propertyName;
		private readonly string value;

		public SearchWord(string val) {
			value = val;
		}

		public SearchWord(string propName, string val) {
			propertyName = propName;
			value = val;
		}

		public string PropertyName {
			get { return propertyName; }
		}

		public string Value {
			get { return value; }
		}
	}

}
