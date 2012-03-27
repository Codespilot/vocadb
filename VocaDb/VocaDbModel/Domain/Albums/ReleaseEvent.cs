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

		public ReleaseEvent(string description, DateTime date, string name)
			: this() {

			ParamIs.NotNullOrEmpty(() => name);

			Description = description;
			Date = date;
			Name = name;

		}

		public ReleaseEvent(string description, DateTime date, ReleaseEventSeries series, int seriesNumber)
			: this() {

			ParamIs.NotNull(() => series);

			Description = description;
			Date = date;
			Series = series;
			SeriesNumber = seriesNumber;
			Name = series.GetEventName(seriesNumber);

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

		public virtual bool Equals(ReleaseEvent another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEvent);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return string.Format("release event '{0}' [{1}]", Name, Id);
		}

	}
}
