using System;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : IEquatable<Tag> {

		public static readonly Regex TagNameRegex = new Regex(@"[\w]+");

		public virtual string Name { get; set; }

		public virtual bool Equals(Tag tag) {

			if (tag == null)
				return false;

			return tag.Name.Equals(Name);

		}

		public override bool Equals(object obj) {
			return Equals(obj as Tag);
		}

		public override int GetHashCode() {
			return Name.GetHashCode();
		}

	}

}
