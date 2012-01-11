using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInListEditContract {

		public SongInListEditContract() { }

		public SongInListEditContract(SongInList songInList, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => songInList);

			SongInListId = songInList.Id;
			Order = songInList.Order;
			SongName = songInList.Song.TranslatedName[languagePreference];
			SongAdditionalNames = string.Join(", ", songInList.Song.AllNames.Where(n => n != SongName));
			SongArtistString = songInList.Song.ArtistString[languagePreference];
			SongId = songInList.Song.Id;

		}

		public SongInListEditContract(SongWithAdditionalNamesContract songContract) {

			ParamIs.NotNull(() => songContract);

			SongId = songContract.Id;
			SongName = songContract.Name;
			SongAdditionalNames = songContract.AdditionalNames;
			SongArtistString = songContract.ArtistString;

		}

		public int Order { get; set; }

		public string SongAdditionalNames { get; set; }

		public string SongArtistString { get; set; }

		public int SongId { get; set; }

		public int SongInListId { get; set; }

		public string SongName { get; set; }

	}
}
