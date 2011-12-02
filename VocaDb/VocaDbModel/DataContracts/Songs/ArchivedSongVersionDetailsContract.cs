using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Songs {

	public class ArchivedSongVersionDetailsContract : ArchivedSongVersionContract {

		public ArchivedSongVersionDetailsContract() { }

		public ArchivedSongVersionDetailsContract(ArchivedSongVersion archived, ContentLanguagePreference languagePreference)
			: base(archived) {

			Song = (archived.Song != null ? new SongContract(archived.Song, languagePreference) : null);
			Data = XmlHelper.DeserializeFromXml<ArchivedSongContract>(archived.Data);

			if (Song != null) {
				Name = Song.Name;
			} else if (Data.TranslatedName != null) {

				var translatedName = new TranslatedString(Data.TranslatedName);
				Name = translatedName[languagePreference];

			}

		}

		public ArchivedSongContract Data { get; set; }

		public string Name { get; set; }

		public SongContract Song { get; set; }

	}

}
