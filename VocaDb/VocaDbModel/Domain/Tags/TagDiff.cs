﻿using System;
using System.Linq;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Tags {

	public class TagDiff : IEntryDiff {

		private void Set(TagEditableFields field, bool val) {

			if (val && !IsChanged(field))
				ChangedFields |= field;
			else if (!val && IsChanged(field))
				ChangedFields -= field;

		}

		public TagDiff() {
			IsSnapshot = true;
		}

		public virtual bool AliasedTo {
			get {
				return IsChanged(TagEditableFields.AliasedTo);
			}
			set {
				Set(TagEditableFields.AliasedTo, value);
			}
		}

		public virtual bool CategoryName {
			get {
				return IsChanged(TagEditableFields.CategoryName);
			}
			set {
				Set(TagEditableFields.CategoryName, value);
			}
		}

		public virtual string[] ChangedFieldNames {
			get {

				var fieldNames = EnumVal<TagEditableFields>.Values
					.Where(f => f != TagEditableFields.Nothing && IsChanged(f)).Select(f => f.ToString());

				return fieldNames.ToArray();

			}
		}

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<TagEditableFields>.Values.Where(f => f != TagEditableFields.Nothing && IsChanged(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = TagEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					TagEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual TagEditableFields ChangedFields { get; set; }

		public virtual bool Description {
			get {
				return IsChanged(TagEditableFields.Description);
			}
			set {
				Set(TagEditableFields.Description, value);
			}		
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Picture {
			get {
				return IsChanged(TagEditableFields.Picture);
			}
			set {
				Set(TagEditableFields.Picture, value);
			}
		}

		public virtual bool Status {
			get {
				return IsChanged(TagEditableFields.Status);
			}
			set {
				Set(TagEditableFields.Status, value);
			}
		}

		/// <summary>
		/// Checks whether a specific field changed in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field was changed, otherwise false.</returns>
		public bool IsChanged(TagEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

	}
}
