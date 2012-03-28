using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesContract {

		public ReleaseEventSeriesContract() {
			Description = string.Empty;
		}

		public ReleaseEventSeriesContract(ReleaseEventSeries series)
			: this() {

			ParamIs.NotNull(() => series);

			Description = series.Description;
			Id = series.Id;
			Name = series.Name;

		}

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

	}

}
