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

			Song = new SongContract(archived.Song, languagePreference);
			Data = ArchivedSongContract.GetAllProperties(archived);
			Name = Song.Name;

		}

		public ArchivedSongContract Data { get; set; }

		public string Name { get; set; }

		public SongContract Song { get; set; }

	}

}
