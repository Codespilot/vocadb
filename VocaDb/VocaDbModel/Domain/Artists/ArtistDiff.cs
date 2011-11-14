using System;
using System.Linq;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistDiff {

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

		public bool IncludePicture { get; set; }

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Picture {
			get {
				return IsSet(ArtistEditableFields.Picture);
			}
			set {
				Set(ArtistEditableFields.Picture, value);
			}
		}

	}

}
