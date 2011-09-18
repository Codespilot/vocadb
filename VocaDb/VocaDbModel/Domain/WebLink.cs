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

		/// <summary>
		/// User-visible link description. Cannot be null.
		/// </summary>
		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		/// <summary>
		/// Link description if the description is not empty. Otherwise URL.
		/// </summary>
		public virtual string DescriptionOrUrl {
			get {
				return !string.IsNullOrEmpty(Description) ? Description : Url;
			}
		}

		public virtual int Id { get; protected set; }

		/// <summary>
		/// Link URL. Cannot be null or empty.
		/// </summary>
		public virtual string Url {
			get { return url; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				url = value;
			}
		}

	}

}
