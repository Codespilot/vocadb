using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArchivedArtistVersionDetailsContract : ArchivedArtistVersionContract {

		public ArchivedArtistVersionDetailsContract() { }

		public ArchivedArtistVersionDetailsContract(ArchivedArtistVersion archived, ContentLanguagePreference languagePreference)
			: base(archived) {

			Artist = (archived.Artist != null ? new ArtistContract(archived.Artist, languagePreference) : null);
			Data = XmlHelper.DeserializeFromXml<ArchivedArtistContract>(archived.Data);

			if (Artist != null) {
				Name = Artist.Name;
			} else if (Data.TranslatedName != null) {

				var translatedName = new TranslatedString(Data.TranslatedName);
				Name = translatedName[languagePreference];

			}

		}

		public ArtistContract Artist { get; set; }

		public ArchivedArtistContract Data { get; set; }

		public string Name { get; set; }

	}

}
