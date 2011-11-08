using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Tags {

	public abstract class TagUsage {

		private Tag tag;
		private IList<TagVote> votes = new List<TagVote>();

		public TagUsage() { }

		public TagUsage(Tag tag) {
			Tag = tag;
		}

		public virtual int Count { get; set; }

		public abstract IEntryBase Entry { get; }

		public virtual int Id { get; set; }

		public virtual Tag Tag {
			get { return tag; }
			set {
				ParamIs.NotNull(() => value);
				tag = value;
			}
		}

		public virtual bool Equals(TagUsage another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as TagUsage);
		}

		public virtual int GetHashCode() {
			return (Tag.Name + "_" + Entry.EntryType + Entry.Id).GetHashCode();
		}

	}

}
