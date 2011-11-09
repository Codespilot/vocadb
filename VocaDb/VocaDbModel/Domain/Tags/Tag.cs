using System;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : IEquatable<Tag> {

		public static readonly Regex TagNameRegex = new Regex(@"^[\w]+$");

		private Iesi.Collections.Generic.ISet<AlbumTagUsage> albumTagUsages = new Iesi.Collections.Generic.HashedSet<AlbumTagUsage>();

		public Tag() { }

		public Tag(string name) {

			if (!TagNameRegex.IsMatch(name))
				throw new ArgumentException("Tag name must contain only word characters", "name");

			Name = name;

		}

		public virtual Iesi.Collections.Generic.ISet<AlbumTagUsage> AlbumTagUsages {
			get { return albumTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				albumTagUsages = value;
			}
		}

		public virtual string Name { get; set; }

		public virtual bool Equals(Tag tag) {

			if (tag == null)
				return false;

			return tag.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);

		}

		public override bool Equals(object obj) {
			return Equals(obj as Tag);
		}

		public override int GetHashCode() {
			return Name.ToLowerInvariant().GetHashCode();
		}

	}

}
