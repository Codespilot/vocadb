using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags {

	public class TagVote {

		private TagUsage tagUsage;
		private User user;

		public virtual TagUsage Usage {
			get { return tagUsage; }
			set {
				ParamIs.NotNull(() => value);
				tagUsage = value;
			}
		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

	}
}
