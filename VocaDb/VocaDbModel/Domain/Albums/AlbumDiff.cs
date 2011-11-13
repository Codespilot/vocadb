using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumDiff {

		public AlbumDiff()
			: this(true) { }

		public AlbumDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public bool Cover { get; set; }

		public bool Description { get; set; }

		public bool IncludeCover {
			get {
				// Special treatment for cover - not included in snapshots by default.
				return Cover;
			}
		}

		public bool IncludeDescription {
			get {
				return (IsSnapshot || Description);
			}
		}

		public bool IncludeNames {
			get {
				return (IsSnapshot || Names);
			}
		}

		public bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public bool IsSnapshot { get; set; }

		public bool Names { get; set; }

		public bool WebLinks { get; set; }

		public string GetChangeString() {

			var items = new List<string>();

			if (Cover)
				items.Add("cover");

			if (Description)
				items.Add("description");

			if (Names)
				items.Add("names");

			if (WebLinks)
				items.Add("weblinks");

			if (items.Any())
				return string.Join(", ", items);
			else
				return "other";

		}

	}


}
