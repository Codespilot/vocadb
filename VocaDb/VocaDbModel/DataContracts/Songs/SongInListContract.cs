using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInListContract {

		public SongInListContract() { }

		public SongInListContract(SongInList songInList, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => songInList);

			Id = songInList.Id;
			Order = songInList.Order;
			Song = new SongWithAdditionalNamesContract(songInList.Song, languagePreference);

		}

		public SongInListContract(Song song, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => song);

			Id = song.Id;
			Song = new SongWithAdditionalNamesContract(song, languagePreference);

		}

		public int Id { get; set; }

		public int Order { get; set; }

		public SongWithAdditionalNamesContract Song { get; set; }

	}
}
