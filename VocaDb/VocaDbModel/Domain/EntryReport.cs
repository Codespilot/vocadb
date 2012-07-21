using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain {

	public abstract class EntryReport {
		
		private string hostname;
		private string notes;

		public virtual DateTime Created { get; set; }

		public virtual EntryType EntryType { get; set; }

		public virtual string Hostname {
			get { return hostname; }
			set { hostname = value; }
		}

		public virtual string Notes {
			get { return notes; }
			set {
				ParamIs.NotNull(() => value);
				notes = value;
			}
		}

		public virtual User User { get; set; }

	}
}
