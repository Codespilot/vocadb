using System;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumReleaseContract {

		public AlbumReleaseContract() {}

		public AlbumReleaseContract(AlbumRelease release, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => release);

			CatNum = release.CatNum;
			ReleaseYear = release.ReleaseYear;
			EventName = release.EventName;
			Label = (release.Label != null ? new ArtistContract(release.Label, languagePreference) : null);

		}

		public string CatNum { get; set; }

		public int? ReleaseYear { get; set; }

		public string EventName { get; set; }

		public ArtistContract Label { get; set; }

	}

}
