using System;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventDetailsContract {

		public ReleaseEventDetailsContract() {
			Date = DateTime.Now;
			Description = string.Empty;
		}

		public ReleaseEventDetailsContract(ReleaseEvent releaseEvent) {

			ParamIs.NotNull(() => releaseEvent);

			Date = releaseEvent.Date;
			Description = releaseEvent.Description;
			Id = releaseEvent.Id;
			Name = releaseEvent.Name;
			Series = (releaseEvent.Series != null ? new ReleaseEventSeriesContract(releaseEvent.Series) : null);
			SeriesNumber = releaseEvent.SeriesNumber;

		}

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public DateTime Date { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public int SeriesNumber { get; set; }

	}
}
