using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	public class TrackPropertiesContract {

		public TrackPropertiesContract() { }

		public TrackPropertiesContract(Song song, IEnumerable<Artist> artists, ContentLanguagePreference languagePreference) {

			Id = song.Id;
			Name = song.TranslatedName[languagePreference];

			ArtistSelections = artists.Select(a => new ArtistSelectionForTrackContract(a, song.HasArtist(a), languagePreference)).ToArray();

		}

		public ArtistSelectionForTrackContract[] ArtistSelections { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

	}
}
