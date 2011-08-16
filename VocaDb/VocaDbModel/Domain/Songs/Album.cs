using System;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class Album {

		public Album() {
			LocalizedName = new LocalizedString();
		}

		public virtual int Id { get; set; }

		public virtual LocalizedString LocalizedName { get; set; }

		public virtual string Name {
			get {
				return LocalizedName.Current;
			}
			set {
				LocalizedName.Current = value;
			}
		}

		public virtual DateTime ReleaseDate { get; set; }

	}

}
