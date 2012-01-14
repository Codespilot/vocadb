using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInListContract {

		public SongInListContract(SongInList songInList, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => songInList);

			Order = songInList.Order;
			Song = new SongWithAdditionalNamesContract(songInList.Song, languagePreference);

		}

		public int Order { get; set; }

		public SongWithAdditionalNamesContract Song { get; set; }

	}

}
