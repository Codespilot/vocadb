using System;

namespace VocaDb.Model.Domain.Albums {

	public class ReleaseEvent {

		private string description;
		private string name;
		private ReleaseEventSeries series;

		public ReleaseEvent() {
			Date = DateTime.Now;
			Description = string.Empty;
		}

		public ReleaseEvent(string name)
			: this() {

			Name = name;

		}

		public virtual DateTime Date { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrWhiteSpace(() => value);
				name = value; 
			}
		}

		public virtual ReleaseEventSeries Series {
			get { return series; }
			set { series = value; }
		}

		public virtual int SeriesNumber { get; set; }

	}
}
