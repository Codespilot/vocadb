using System;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventDetailsContract : ReleaseEventContract {

		public ReleaseEventDetailsContract() {}

		public ReleaseEventDetailsContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference) 
			: base(releaseEvent) {

			ParamIs.NotNull(() => releaseEvent);

			Albums = releaseEvent.Albums.Select(a => new AlbumWithAdditionalNamesContract(a, languagePreference)).ToArray();
			Series = (releaseEvent.Series != null ? new ReleaseEventSeriesContract(releaseEvent.Series) : null);
			SeriesNumber = releaseEvent.SeriesNumber;

		}

		public AlbumWithAdditionalNamesContract[] Albums { get; set; }

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public int SeriesNumber { get; set; }

	}
}
