using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumTagUsageContract : TagUsageContract {

		public AlbumTagUsageContract() { }

		public AlbumTagUsageContract(AlbumTagUsage tagUsage, ContentLanguagePreference languagePreference)
			: base(tagUsage) {

			Album = new AlbumWithAdditionalNamesContract(tagUsage.Album, languagePreference);

		}

		public AlbumWithAdditionalNamesContract Album { get; set; } 

	}

}
