namespace VocaDb.Model.Domain.Albums {

	public class ReleaseEventSeriesAlias {

		private string name;
		private ReleaseEventSeries series;

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value; 
			}
		}

		public virtual ReleaseEventSeries Series {
			get { return series; }
			set {
				ParamIs.NotNull(() => value);
				series = value; 
			}
		}

	}
}
