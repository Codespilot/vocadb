using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArchivedArtistVersionDetailsContract : ArchivedArtistVersionContract {

		public ArchivedArtistVersionDetailsContract() { }

		public ArchivedArtistVersionDetailsContract(ArchivedArtistVersion archived, ContentLanguagePreference languagePreference)
			: base(archived) {

			Artist = new ArtistContract(archived.Artist, languagePreference);
			Data = ArchivedArtistContract.GetAllProperties(archived);
			Name = Artist.Name;

		}

		public ArtistContract Artist { get; set; }

		public ArchivedArtistContract Data { get; set; }

		public string Name { get; set; }

	}

}
