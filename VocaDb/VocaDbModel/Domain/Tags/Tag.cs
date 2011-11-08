using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : IEquatable<Tag> {

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
