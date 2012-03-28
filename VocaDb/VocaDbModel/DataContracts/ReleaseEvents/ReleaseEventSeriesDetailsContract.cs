using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesDetailsContract : ReleaseEventSeriesWithEventsContract {

		public ReleaseEventSeriesDetailsContract() { }

		public ReleaseEventSeriesDetailsContract(ReleaseEventSeries series)
			: base(series) {

			Aliases = series.Aliases.Select(a => a.Name).ToArray();

		}

		public string[] Aliases { get; set; }

	}
}
