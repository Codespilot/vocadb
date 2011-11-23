using System;
using System.Linq;

namespace VocaDb.Model.Domain.Songs {

	public class SongDiff {

		private bool IsSet(SongEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

		private void Set(SongEditableFields field, bool val) {

			if (val && !IsSet(field))
				ChangedFields |= field;
			else if (!val && IsSet(field))
				ChangedFields -= field;

		}

		public SongDiff()
			: this(true) { }

		public SongDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public virtual bool Artists {
			get {
				return IsSet(SongEditableFields.Artists);
			}
			set {
				Set(SongEditableFields.Artists, value);
			}
		}

		public virtual SongEditableFields ChangedFields { get; set; }

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<SongEditableFields>.Values.Where(f => f != SongEditableFields.Nothing && IsSet(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = SongEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					SongEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual bool IncludeArtists {
			get {
				return true;
			}
		}

		public bool IncludeLyrics {
			get {
				return (IsSnapshot || Lyrics);
			}
		}

		public virtual bool IncludeNames {
			get {
				return (IsSnapshot || Names);
			}
		}

		public virtual bool IncludePVs {
			get {
				return true;
			}
		}

		public virtual bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Lyrics {
			get {
				return IsSet(SongEditableFields.Lyrics);
			}
			set {
				Set(SongEditableFields.Lyrics, value);
			}
		}

		public virtual bool Names {
			get {
				return IsSet(SongEditableFields.Names);
			}
			set {
				Set(SongEditableFields.Names, value);
			}
		}

		public virtual bool Notes {
			get {
				return IsSet(SongEditableFields.Notes);
			}
			set {
				Set(SongEditableFields.Notes, value);
			}
		}

		public virtual bool OriginalName {
			get {
				return IsSet(SongEditableFields.OriginalName);
			}
			set {
				Set(SongEditableFields.OriginalName, value);
			}
		}

		public virtual bool OriginalVersion {
			get {
				return IsSet(SongEditableFields.OriginalVersion);
			}
			set {
				Set(SongEditableFields.OriginalVersion, value);
			}
		}

		public virtual bool PVs {
			get {
				return true;
			}
		}

		public virtual bool SongType {
			get {
				return IsSet(SongEditableFields.SongType);
			}
			set {
				Set(SongEditableFields.SongType, value);
			}
		}

		public virtual bool WebLinks {
			get {
				return IsSet(SongEditableFields.WebLinks);
			}
			set {
				Set(SongEditableFields.WebLinks, value);
			}
		}


	}

}
