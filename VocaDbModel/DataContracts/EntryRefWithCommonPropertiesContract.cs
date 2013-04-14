using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Resources;
using VocaDb.Model.Resources.Albums;
using VocaDb.Model.Resources.Songs;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Entry reference with entry title and (translated) entry type name.
	/// </summary>
	public class EntryRefWithCommonPropertiesContract : EntryRefWithNameContract {

		public EntryRefWithCommonPropertiesContract(Album entry, ContentLanguagePreference languagePreference)
			: base(entry, languagePreference) {

			//ArtistString = entry.ArtistString[languagePreference];
			EntryTypeName = DiscTypeNames.ResourceManager.GetString(entry.DiscType.ToString());

		}

		public EntryRefWithCommonPropertiesContract(Artist entry, ContentLanguagePreference languagePreference)
			: base(entry, languagePreference) {

			//ArtistString = null;
			EntryTypeName = ArtistTypeNames.ResourceManager.GetString(entry.ArtistType.ToString());

		}

		public EntryRefWithCommonPropertiesContract(Song entry, ContentLanguagePreference languagePreference)
			: base(entry, languagePreference) {

			//ArtistString = entry.ArtistString[languagePreference];
			EntryTypeName = SongTypeNames.ResourceManager.GetString(entry.SongType.ToString());

		}

		//public string ArtistString { get; set; }

		/// <summary>
		/// Translated entry type name.
		/// </summary>
		public string EntryTypeName { get; set; }

	}

}
