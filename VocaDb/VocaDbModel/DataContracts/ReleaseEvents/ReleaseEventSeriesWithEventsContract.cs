using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesWithEventsContract : ReleaseEventSeriesContract {

		public ReleaseEventSeriesWithEventsContract() {}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series) : base(series) {

			Events = series.Events.Select(e => new ReleaseEventDetailsContract(e)).ToArray();
		
		}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, IEnumerable<ReleaseEvent> events)
			: base(series) {

			ParamIs.NotNull(() => events);

			Events = events.Select(e => new ReleaseEventDetailsContract(e)).ToArray();

		}

		public ReleaseEventDetailsContract[] Events { get; set; }

	}
}
