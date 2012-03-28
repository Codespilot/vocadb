using System;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventDetailsContract {

		public ReleaseEventDetailsContract() {
			Date = DateTime.Now;
			Description = string.Empty;
		}

		public ReleaseEventDetailsContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => releaseEvent);

			Albums = releaseEvent.Albums.Select(a => new AlbumWithAdditionalNamesContract(a, languagePreference)).ToArray();
			Date = releaseEvent.Date;
			Description = releaseEvent.Description;
			Id = releaseEvent.Id;
			Name = releaseEvent.Name;
			Series = (releaseEvent.Series != null ? new ReleaseEventSeriesContract(releaseEvent.Series) : null);
			SeriesNumber = releaseEvent.SeriesNumber;

		}

		public AlbumWithAdditionalNamesContract[] Albums { get; set; }

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public DateTime Date { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public int SeriesNumber { get; set; }

	}
}
