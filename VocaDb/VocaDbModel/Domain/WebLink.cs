namespace VocaDb.Model.Domain {

	public class WebLink {

		private string description;
		private string url;

		public string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public string Url {
			get { return url; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				url = value;
			}
		}

	}

}
