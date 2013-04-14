using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistWithAdditionalNamesContract : ArtistContract {

		public ArtistWithAdditionalNamesContract() {}

		public ArtistWithAdditionalNamesContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

		}

	}

}
