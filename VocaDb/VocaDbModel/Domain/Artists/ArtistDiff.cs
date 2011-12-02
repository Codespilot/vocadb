using System;
using System.Linq;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistDiff : IEntryDiff {

		private bool IsSet(ArtistEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

		private void Set(ArtistEditableFields field, bool val) {

			if (val && !IsSet(field))
				ChangedFields |= field;
			else if (!val && IsSet(field))
				ChangedFields -= field;

		}

		public ArtistDiff()
			: this(true) { }

		public ArtistDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public virtual bool Albums {
			get {
				return IsSet(ArtistEditableFields.Albums);
			}
			set {
				Set(ArtistEditableFields.Albums, value);
			}
		}

		public virtual bool ArtistType {
			get {
				return IsSet(ArtistEditableFields.ArtistType);
			}
			set {
				Set(ArtistEditableFields.ArtistType, value);
			}
		}

		public virtual ArtistEditableFields ChangedFields { get; set; }

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<ArtistEditableFields>.Values.Where(f => f != ArtistEditableFields.Nothing && IsSet(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = ArtistEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					ArtistEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual bool Description {
			get {
				return IsSet(ArtistEditableFields.Description);
			}
			set {
				Set(ArtistEditableFields.Description, value);
			}
		}

		public virtual bool Groups {
			get {
				return IsSet(ArtistEditableFields.Groups);
			}
			set {
				Set(ArtistEditableFields.Groups, value);
			}
		}

		public virtual bool IncludeAlbums {
			get {
				return (IsSnapshot || Albums);
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

		public virtual bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public bool IncludePicture {
			get {
				// Special handling for pictures
				return Picture;
			}
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Names {
			get {
				return IsSet(ArtistEditableFields.Names);
			}
			set {
				Set(ArtistEditableFields.Names, value);
			}
		}

		public virtual bool OriginalName {
			get {
				return IsSet(ArtistEditableFields.OriginalName);
			}
			set {
				Set(ArtistEditableFields.OriginalName, value);
			}
		}

		public virtual bool Picture {
			get {
				return IsSet(ArtistEditableFields.Picture);
			}
			set {
				Set(ArtistEditableFields.Picture, value);
			}
		}

		public virtual bool Status {
			get {
				return IsSet(ArtistEditableFields.Status);
			}
			set {
				Set(ArtistEditableFields.Status, value);
			}
		}

		public virtual bool WebLinks {
			get {
				return IsSet(ArtistEditableFields.WebLinks);
			}
			set {
				Set(ArtistEditableFields.WebLinks, value);
			}
		}

	}

}
