using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongTagUsageContract : TagUsageContract {

		public SongTagUsageContract() { }

		public SongTagUsageContract(SongTagUsage tagUsage, ContentLanguagePreference languagePreference)
			: base(tagUsage) {

			Song = new SongContract(tagUsage.Song, languagePreference);

		}

		public SongContract Song { get; set; } 

	}

}
