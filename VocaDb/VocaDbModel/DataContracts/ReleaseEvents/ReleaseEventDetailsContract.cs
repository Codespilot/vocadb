﻿using System;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventDetailsContract : ReleaseEventContract {

		public ReleaseEventDetailsContract() {}

		public ReleaseEventDetailsContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference) 
			: base(releaseEvent, true) {

			ParamIs.NotNull(() => releaseEvent);

			Albums = releaseEvent.Albums.Where(a => !a.Deleted).Select(a => new AlbumWithAdditionalNamesContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			SeriesNumber = releaseEvent.SeriesNumber;
			SeriesSuffix = releaseEvent.SeriesSuffix;

		}

		public AlbumWithAdditionalNamesContract[] Albums { get; set; }

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public int SeriesNumber { get; set; }

		public string SeriesSuffix { get; set; }

	}
}
