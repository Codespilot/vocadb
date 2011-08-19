using System;
using System.Collections.Generic;

namespace VocaDb.Model.Domain.Artists {

	public class Circle : Artist {

		private IList<Artist> members = new List<Artist>();

		public virtual IList<Artist> CircleMembers {
			get { return members; }
			set {
				ParamIs.NotNull(() => value);
				members = value;
			}
		}

		public override IEnumerable<Artist> Members {
			get { return CircleMembers; }
		}

	}
}
