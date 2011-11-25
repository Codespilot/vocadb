using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;
using System;

namespace VocaDb.Model.Domain {

	public class WebLink {

		public static CollectionDiffWithValue<T,T> Sync<T>(IList<T> oldLinks, IEnumerable<WebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory) 
			where T : WebLink {

			var diff = CollectionHelper.Diff(oldLinks, newLinks, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed) {
				oldLinks.Remove(n);
			}

			foreach (var linkEntry in newLinks) {

				var entry = linkEntry;
				var old = (entry.Id != 0 ? oldLinks.FirstOrDefault(n => n.Id == entry.Id) : null);

				if (old != null) {

					if (old.Description != linkEntry.Description || old.Url != linkEntry.Url) {
						old.Description = linkEntry.Description;
						old.Url = linkEntry.Url;
						edited.Add(old);
					}

				} else {

					var n = webLinkFactory.CreateWebLink(linkEntry.Description, linkEntry.Url);
					created.Add(n);

				}

			}

			return new CollectionDiffWithValue<T, T>(created, diff.Removed, diff.Unchanged, edited);

		}

		private string description;
		private string url;

		public WebLink() {}

		public WebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrWhiteSpace(() => url);

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
				ParamIs.NotNullOrWhiteSpace(() => value);
				url = value;
			}
		}

		public virtual bool ContentEquals(WebLink another) {

			if (another == null)
				return false;

			return (Url == another.Url && Description == another.Description);

		}

		public override string ToString() {
			return "web link '" + Url + "'";
		}

	}

}
