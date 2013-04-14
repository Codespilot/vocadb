using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistTagUsageContract : TagUsageContract {

		public ArtistTagUsageContract() { }

		public ArtistTagUsageContract(ArtistTagUsage tagUsage, ContentLanguagePreference languagePreference)
			: base(tagUsage) {

			Artist = new ArtistWithAdditionalNamesContract(tagUsage.Artist, languagePreference);

		}

		public ArtistWithAdditionalNamesContract Artist { get; set; }

	}
}
