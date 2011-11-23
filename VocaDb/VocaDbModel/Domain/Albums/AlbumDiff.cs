using System.Linq;
using System;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumDiff {

		private bool IsSet(AlbumEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

		private void Set(AlbumEditableFields field, bool val) {

			if (val && !IsSet(field))
				ChangedFields |= field;
			else if (!val && IsSet(field))
				ChangedFields -= field;

		}

		public AlbumDiff()
			: this(true) { }

		public AlbumDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public virtual bool Artists {
			get {
				return IsSet(AlbumEditableFields.Artists);
			}
			set {
				Set(AlbumEditableFields.Artists, value);
			}
		}

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<AlbumEditableFields>.Values.Where(f => f != AlbumEditableFields.Nothing && IsSet(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = AlbumEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					AlbumEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual AlbumEditableFields ChangedFields { get; set; }

		public virtual bool Cover {
			get {
				return IsSet(AlbumEditableFields.Cover);
			}
			set {
				Set(AlbumEditableFields.Cover, value);
			}
		}

		public virtual bool Description {
			get {
				return IsSet(AlbumEditableFields.Description);
			}
			set {
				Set(AlbumEditableFields.Description, value);
			}		
		}

		public virtual bool DiscType {
			get {
				return IsSet(AlbumEditableFields.DiscType);
			}
			set {
				Set(AlbumEditableFields.DiscType, value);
			}
		}

		public virtual bool IncludeArtists {
			get {
				return (IsSnapshot || Artists);
			}
		}

		public virtual bool IncludeCover {
			get {
				// Special treatment for cover - not included in snapshots by default.
				return Cover;
			}
		}

		public virtual bool IncludeDescription {
			get {
				return (IsSnapshot || Description);
			}
		}

		public virtual bool IncludeNames {
			get {
				return (IsSnapshot || Names);
			}
		}

		public virtual bool IncludeTracks {
			get {
				return (IsSnapshot || Tracks);
			}
		}

		public virtual bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Names {
			get {
				return IsSet(AlbumEditableFields.Names);
			}
			set {
				Set(AlbumEditableFields.Names, value);
			}
		}

		public virtual bool OriginalName {
			get {
				return IsSet(AlbumEditableFields.OriginalName);
			}
			set {
				Set(AlbumEditableFields.OriginalName, value);
			}
		}

		public virtual bool OriginalRelease {
			get {
				return IsSet(AlbumEditableFields.OriginalRelease);
			}
			set {
				Set(AlbumEditableFields.OriginalRelease, value);
			}
		}

		public virtual bool Status {
			get {
				return IsSet(AlbumEditableFields.Status);
			}
			set {
				Set(AlbumEditableFields.Status, value);
			}
		}

		public virtual bool Tracks {
			get {
				return IsSet(AlbumEditableFields.Tracks);
			}
			set {
				Set(AlbumEditableFields.Tracks, value);
			}
		}

		public virtual bool WebLinks {
			get {
				return IsSet(AlbumEditableFields.WebLinks);
			}
			set {
				Set(AlbumEditableFields.WebLinks, value);
			}		
		}

	}


}
