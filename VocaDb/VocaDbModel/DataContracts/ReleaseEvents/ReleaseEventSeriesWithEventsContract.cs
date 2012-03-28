using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesWithEventsContract : ReleaseEventSeriesContract {

		public ReleaseEventSeriesWithEventsContract() {}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference)
			: base(series) {

			Events = series.Events.Select(e => new ReleaseEventDetailsContract(e, languagePreference)).ToArray();
		
		}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, IEnumerable<ReleaseEvent> events, ContentLanguagePreference languagePreference)
			: base(series) {

			ParamIs.NotNull(() => events);

			Events = events.Select(e => new ReleaseEventDetailsContract(e, languagePreference)).ToArray();

		}

		public ReleaseEventDetailsContract[] Events { get; set; }

	}
}
