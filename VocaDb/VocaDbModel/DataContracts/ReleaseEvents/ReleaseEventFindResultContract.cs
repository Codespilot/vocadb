using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventFindResultContract {

		public ReleaseEventFindResultContract() { }

		public ReleaseEventFindResultContract(ReleaseEvent releaseEvent) {

			ParamIs.NotNull(() => releaseEvent);

			EventId = releaseEvent.Id;
			EventName = releaseEvent.Name;

		}

		public ReleaseEventFindResultContract(string eventName) {

			EventName = eventName;

		}

		public ReleaseEventFindResultContract(ReleaseEventSeries series, int seriesNumber, string eventName) {

			Series = new ReleaseEventSeriesContract(series);
			SeriesNumber = seriesNumber;
			EventName = eventName;

		}

		public int EventId { get; set; }

		public string EventName { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public int SeriesNumber { get; set; }

	}

}
