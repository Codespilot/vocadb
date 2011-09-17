namespace VocaDb.Model.Domain {

	public class WebLink {

		private string description;
		private string url;

		public WebLink() {}

		public WebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			Description = description;
			Url = url;

		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual int Id { get; protected set; }

		public virtual string Url {
			get { return url; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				url = value;
			}
		}

	}

}
