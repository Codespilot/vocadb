using System;
using System.Collections.Generic;

namespace VocaDb.Model.Domain.Albums {

	public class ReleaseEventSeries : IEquatable<ReleaseEventSeries> {

		private IList<ReleaseEventSeriesAlias> aliases = new List<ReleaseEventSeriesAlias>();
		private string description;
		private IList<ReleaseEvent> events = new List<ReleaseEvent>();
		private string name;

		public ReleaseEventSeries() {
			Description = string.Empty;
		}

		public virtual IList<ReleaseEventSeriesAlias> Aliases {
			get { return aliases; }
			set {
				ParamIs.NotNull(() => value);
				aliases = value; 
			}
		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public virtual IList<ReleaseEvent> Events {
			get { return events; }
			set {
				ParamIs.NotNull(() => value);
				events = value; 
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value; 
			}
		}

		public virtual bool Equals(ReleaseEventSeries another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEventSeries);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}

}
