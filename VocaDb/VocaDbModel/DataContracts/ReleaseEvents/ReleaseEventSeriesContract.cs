using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesContract {

		public ReleaseEventSeriesContract() { }

		public ReleaseEventSeriesContract(ReleaseEventSeries series) {

			ParamIs.NotNull(() => series);

			Description = series.Description;
			Name = series.Name;

		}

		public string Description { get; set; }

		public string Name { get; set; }

	}

}
